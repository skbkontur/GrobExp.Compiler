using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using GrEmit;

using GrobExp.Compiler.Closures;

namespace GrobExp.Compiler
{
    public static class LambdaCompiler
    {
        public static Delegate Compile(LambdaExpression lambda, CompilerOptions options)
        {
            CheckLambdaParameters(lambda);
            CompiledLambda[] subLambdas;
            var debugInfoGenerator = string.IsNullOrEmpty(DebugOutputDirectory) ? null : DebugInfoGenerator.CreatePdbGenerator();
            return CompileInternal(lambda, debugInfoGenerator, out subLambdas, options);
        }

        public static TDelegate Compile<TDelegate>(Expression<TDelegate> lambda, CompilerOptions options) where TDelegate : class
        {
            CheckLambdaParameters(lambda);
            CompiledLambda[] subLambdas;
            var debugInfoGenerator = string.IsNullOrEmpty(DebugOutputDirectory) ? null : DebugInfoGenerator.CreatePdbGenerator();
            return (TDelegate)(object)CompileInternal(lambda, debugInfoGenerator, out subLambdas, options);
        }

        public static void CompileToMethod(LambdaExpression lambda, MethodBuilder method, CompilerOptions options)
        {
            CheckLambdaParameters(lambda);
            CompileToMethodInternal(lambda, method, null, options);
        }

        public static void CompileToMethod(LambdaExpression lambda, MethodBuilder method, DebugInfoGenerator debugInfoGenerator, CompilerOptions options)
        {
            CheckLambdaParameters(lambda);
            CompileToMethodInternal(lambda, method, debugInfoGenerator, options);
        }

        private static void CheckLambdaParameters(LambdaExpression lambda)
        {
            var parameters = new ParametersExtractor().Extract(lambda.Body);
            var hashset = new HashSet<ParameterExpression>(lambda.Parameters);
            foreach (var parameter in parameters)
                if (!hashset.Contains(parameter))
                    throw new InvalidOperationException($"Lambda contains parameter not presented in Lambda.Parameters: {parameter}");
        }

        public static bool AnalyzeILStack = true;
        public static string DebugOutputDirectory = null;
        public static double TotalJITCompilationTime = 0;

        internal static CompiledLambda CompileInternal(
            LambdaExpression lambda,
            DebugInfoGenerator debugInfoGenerator,
            ParsedLambda parsedLambda,
            CompilerOptions options,
            bool internalCall,
            List<CompiledLambda> compiledLambdas)
        {
            if (debugInfoGenerator == null)
                return CompileToDynamicMethod(lambda, parsedLambda, options, internalCall, compiledLambdas);

            var parameters = lambda.Parameters.ToArray();
            var parameterTypes = parameters.Select(parameter => parameter.Type).ToArray();
            var returnType = lambda.ReturnType;

            var typeBuilder = Module.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Public | TypeAttributes.Class);
            var method = typeBuilder.DefineMethod(lambda.Name ?? Guid.NewGuid().ToString(), MethodAttributes.Static | MethodAttributes.Public, returnType, parameterTypes);
            for (var i = 0; i < parameters.Length; ++i)
                method.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
            CompileToMethodInternal(lambda, debugInfoGenerator, parsedLambda, options, compiledLambdas, method);

            var type = typeBuilder.CreateTypeInfo();
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, Module, true);
            using (var il = new GroboIL(dynamicMethod))
            {
                for (var i = 0; i < parameterTypes.Length; ++i)
                    il.Ldarg(i);
                il.Call(type.GetMethod(method.Name));
                il.Ret();
            }
            return new CompiledLambda
                {
                    Delegate = Extensions.IsMono && internalCall ? dynamicMethod.CreateDelegate(Extensions.GetDelegateType(parameterTypes, returnType))
                                   : dynamicMethod.CreateDelegate(Extensions.GetDelegateType(parsedLambda.ConstantsParameter == null ? parameterTypes : parameterTypes.Skip(1).ToArray(), lambda.ReturnType), parsedLambda.Constants),
                    Method = method
                };
        }

        internal static void CompileToMethodInternal(
            LambdaExpression lambda,
            DebugInfoGenerator debugInfoGenerator,
            ParsedLambda parsedLambda,
            CompilerOptions options,
            List<CompiledLambda> compiledLambdas,
            MethodBuilder method)
        {
            var typeBuilder = method.ReflectedType as TypeBuilder;
            if (typeBuilder == null)
                throw new ArgumentException("Unable to obtain type builder of the method", "method");
            using (var il = new GroboIL(method, AnalyzeILStack))
            {
                var context = new EmittingContext
                    {
                        Options = options,
                        DebugInfoGenerator = debugInfoGenerator,
                        TypeBuilder = typeBuilder,
                        Lambda = lambda,
                        Method = method,
                        SkipVisibility = false,
                        Parameters = lambda.Parameters.ToArray(),
                        ParsedLambda = parsedLambda,
                        CompiledLambdas = compiledLambdas,
                        Il = il
                    };
                CompileInternal(lambda, context);
            }
        }

        internal static readonly AssemblyBuilder Assembly = CreateAssembly();

#if NETSTANDARD2_0
        internal static readonly ModuleBuilder Module = Assembly.DefineDynamicModule(Guid.NewGuid().ToString());
#else
        internal static readonly ModuleBuilder Module = Assembly.DefineDynamicModule(Guid.NewGuid().ToString(), true);
#endif

        private static string GenerateFileName(Expression expression)
        {
            var hash = ExpressionHashCalculator.CalcHashCode(expression, true);
            if (!Directory.Exists(DebugOutputDirectory))
                Directory.CreateDirectory(DebugOutputDirectory);
            return Path.Combine(DebugOutputDirectory, "Z" + Math.Abs(hash) + ".lambda");
        }

        private static void CompileInternal(LambdaExpression lambda, EmittingContext context)
        {
            var returnType = lambda.ReturnType;
            var il = context.Il;
            var returnDefaultValueLabel = context.CanReturn ? il.DefineLabel("returnDefaultValue") : null;
            Type resultType;
            var whatReturn = returnType == typeof(void) ? ResultType.Void : ResultType.Value;
            var labelUsed = ExpressionEmittersCollection.Emit(lambda.Body, context, returnDefaultValueLabel, whatReturn, false, out resultType);
            if (returnType == typeof(bool) && resultType == typeof(bool?))
                context.ConvertFromNullableBoolToBool();
            if (returnType == typeof(void) && resultType != typeof(void))
            {
                using (var temp = context.DeclareLocal(resultType))
                    il.Stloc(temp);
            }
            il.Ret();
            if (!labelUsed)
                return;
            context.MarkLabelAndSurroundWithSP(returnDefaultValueLabel);
            il.Pop();
            if (returnType != typeof(void))
            {
                if (!returnType.IsValueType)
                    il.Ldnull();
                else
                {
                    using (var defaultValue = context.DeclareLocal(returnType))
                    {
                        il.Ldloca(defaultValue);
                        il.Initobj(returnType);
                        il.Ldloc(defaultValue);
                    }
                }
            }
            il.Ret();
        }

        private static CompiledLambda CompileToDynamicMethod(
            LambdaExpression lambda,
            ParsedLambda parsedLambda,
            CompilerOptions options,
            bool internalCall,
            List<CompiledLambda> compiledLambdas)
        {
            var parameters = lambda.Parameters.ToArray();
            var parameterTypes = parameters.Select(parameter => parameter.Type).ToArray();
            var returnType = lambda.ReturnType;
            var method = new DynamicMethod(lambda.Name ?? Guid.NewGuid().ToString(), MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, returnType, parameterTypes, Module, true);
            using (var il = new GroboIL(method, AnalyzeILStack))
            {
                var context = new EmittingContext
                    {
                        Options = options,
                        Lambda = lambda,
                        Method = method,
                        SkipVisibility = true,
                        Parameters = parameters,
                        ParsedLambda = parsedLambda,
                        CompiledLambdas = compiledLambdas,
                        Il = il
                    };
                CompileInternal(lambda, context);
            }
            return new CompiledLambda
                {
                    Delegate = Extensions.IsMono && internalCall ? method.CreateDelegate(Extensions.GetDelegateType(parameterTypes, returnType))
                                   : method.CreateDelegate(Extensions.GetDelegateType(parsedLambda.ConstantsParameter == null ? parameterTypes : parameterTypes.Skip(1).ToArray(), lambda.ReturnType), parsedLambda.Constants),
                    Method = method
                };
        }

        private static AssemblyBuilder CreateAssembly()
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);

//            Type daType = typeof(AssemblyFlagsAttribute);
//            ConstructorInfo daCtor = daType.GetConstructor(new[] {typeof(AssemblyNameFlags)});
////[assembly : AssemblyFlags(AssemblyNameFlags.EnableJITcompileOptimizer)]
//            var daBuilder = new CustomAttributeBuilder(daCtor, new object[]
//                {
//                    AssemblyNameFlags.EnableJITcompileOptimizer
//                });
//            assemblyBuilder.SetCustomAttribute(daBuilder);
            return assemblyBuilder;
        }

        private static Delegate CompileInternal(LambdaExpression lambda, DebugInfoGenerator debugInfoGenerator, out CompiledLambda[] subLambdas, CompilerOptions options)
        {
            var compiledLambdas = new List<CompiledLambda>();
            ParsedLambda parsedLambda;
            var emitToDynamicMethod = debugInfoGenerator == null;
            var resolvedLambda = new ExpressionClosureResolver(lambda, Module, emitToDynamicMethod, options).Resolve(out parsedLambda);
            if (!string.IsNullOrEmpty(DebugOutputDirectory))
            {
                resolvedLambda = AdvancedDebugViewWriter.WriteToModifying(resolvedLambda, parsedLambda.ConstantsType,
                                                                          parsedLambda.ConstantsParameter, parsedLambda.Constants, GenerateFileName(resolvedLambda));
            }
            var compiledLambda = CompileInternal(resolvedLambda, debugInfoGenerator, parsedLambda, options, false, compiledLambdas);
            subLambdas = compiledLambdas.ToArray();
            if (compiledLambdas.Count > 0 && emitToDynamicMethod)
                BuildDelegatesFoister(parsedLambda)(parsedLambda.Constants, compiledLambdas.Select(compIledLambda => compIledLambda.Delegate).ToArray());
            return compiledLambda.Delegate;
        }

        private static void CompileToMethodInternal(LambdaExpression lambda, MethodBuilder method, DebugInfoGenerator debugInfoGenerator, CompilerOptions options)
        {
            var compiledLambdas = new List<CompiledLambda>();
            ParsedLambda parsedLambda;
            var module = method.Module as ModuleBuilder;
            if (module == null)
                throw new ArgumentException("Unable to obtain module builder of the method", "method");
            method.SetReturnType(lambda.ReturnType);
            method.SetParameters(lambda.Parameters.Select(parameter => parameter.Type).ToArray());
            var resolvedLambda = new ExpressionClosureResolver(lambda, module, false, options).Resolve(out parsedLambda);
            if (!string.IsNullOrEmpty(DebugOutputDirectory))
            {
                resolvedLambda = AdvancedDebugViewWriter.WriteToModifying(resolvedLambda, parsedLambda.ConstantsType,
                                                                          parsedLambda.ConstantsParameter, parsedLambda.Constants, GenerateFileName(resolvedLambda));
            }
            if (parsedLambda.ConstantsParameter != null)
                throw new InvalidOperationException("Non-trivial constants are not allowed for compilation to method");
            CompileToMethodInternal(resolvedLambda, debugInfoGenerator, parsedLambda, options, compiledLambdas, method);
        }

        private static Action<object, Delegate[]> BuildDelegatesFoister(ParsedLambda parsedLambda)
        {
            var constants = Expression.Parameter(typeof(object));
            var delegates = Expression.Parameter(typeof(Delegate[]));
            var castedConstants = Expression.Parameter(parsedLambda.ConstantsType);
            var body = Expression.Block(new[] {castedConstants},
                                        Expression.Assign(castedConstants, Expression.Convert(constants, parsedLambda.ConstantsType)),
                                        parsedLambda.ConstantsBuilder.Assign(castedConstants, parsedLambda.DelegatesFieldId, delegates));
            var lambda = Expression.Lambda<Action<object, Delegate[]>>(body, constants, delegates);
            return Compile(lambda, CompilerOptions.None);
        }
    }
}