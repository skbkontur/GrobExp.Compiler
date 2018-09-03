using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    public static class DynamicMethodInvokerBuilder
    {
        public static Type BuildDynamicMethodInvoker(ModuleBuilder module, Type[] constantTypes, Type resultType, Type[] parameterTypes)
        {
            // TODO make module as key (use WeakReference or something like that)
            Type type;
            if (Extensions.IsMono)
                type = MonoSucks.Get(constantTypes, resultType, parameterTypes);
            else
            {
                module = module ?? LambdaCompiler.Module;
                var key = GetKey(module, constantTypes, resultType, parameterTypes);
                type = (Type)types[key];
                if (type == null)
                {
                    lock (typesLock)
                    {
                        type = (Type)types[key];
                        if (type == null)
                        {
                            type = BuildDynamicMethodInvoker(module, constantTypes, parameterTypes, resultType);
                            types[key] = type;
                        }
                    }
                }
            }
            if (!type.IsGenericType)
                return type;
            var genericArguments = new List<Type>();
            genericArguments.AddRange(constantTypes);
            genericArguments.AddRange(parameterTypes);
            if (resultType != typeof(void))
                genericArguments.Add(resultType);
            return type.MakeGenericType(genericArguments.ToArray());
        }

        private static Type BuildDynamicMethodInvoker(ModuleBuilder module, Type[] constantTypes, Type[] parameterTypes, Type resultType)
        {
            bool returnsVoid = resultType == typeof(void);
            string name = (returnsVoid ? "ActionInvoker" : "FuncInvoker") + "_" + Guid.NewGuid();
            var typeBuilder = module.DefineType(name, TypeAttributes.Public | TypeAttributes.Class);
            var names = new List<string>();
            int numberOfConstants = constantTypes.Length;
            int numberOfParameters = parameterTypes.Length;
            if (!Extensions.IsMono)
            {
                for (var i = 0; i < numberOfConstants; ++i)
                    names.Add("TConst" + (i + 1));
                for (var i = 0; i < numberOfParameters; ++i)
                    names.Add("TParam" + (i + 1));
                if (!returnsVoid)
                    names.Add("TResult");
                var genericParameters = typeBuilder.DefineGenericParameters(names.ToArray());
                constantTypes = genericParameters.Take(numberOfConstants).Cast<Type>().ToArray();
                parameterTypes = genericParameters.Skip(numberOfConstants).Take(numberOfParameters).Cast<Type>().ToArray();
                if (!returnsVoid)
                    resultType = genericParameters.Last();
            }
            var methodField = typeBuilder.DefineField("method", typeof(IntPtr), FieldAttributes.Public);
            var constantFields = new List<FieldInfo>();
            for (var i = 0; i < numberOfConstants; ++i)
                constantFields.Add(typeBuilder.DefineField("const_" + (i + 1), constantTypes[i], FieldAttributes.Public));

            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, constantTypes.Concat(new[] {typeof(IntPtr)}).ToArray());
            using (var il = new GroboIL(constructor))
            {
                for (var i = 0; i < numberOfConstants; ++i)
                {
                    il.Ldarg(0); // stack: [this]
                    il.Ldarg(i + 1); // stack: [this, arg_{i+1}]
                    il.Stfld(constantFields[i]); // this.const_{i+1} = arg_{i+1}; stack: []
                }
                il.Ldarg(0); // stack: [this]
                il.Ldarg(numberOfConstants + 1); // stack: [this, arg_{constants + 1} = method]
                il.Stfld(methodField); // this.method = method; stack: []
                il.Ret();
            }

            var method = typeBuilder.DefineMethod("Invoke", MethodAttributes.Public, resultType, parameterTypes);
            using (var il = new GroboIL(method))
            {
                for (var i = 0; i < numberOfConstants; ++i)
                {
                    il.Ldarg(0); // stack: [this]
                    il.Ldfld(constantFields[i]); // stack: [this.const_{i+1}]
                }
                for (var i = 0; i < numberOfParameters; ++i)
                    il.Ldarg(i + 1);
                il.Ldarg(0);
                il.Ldfld(methodField);
                il.Calli(CallingConventions.Standard, resultType, constantTypes.Concat(parameterTypes).ToArray());
                il.Ret();
            }

            return typeBuilder.CreateTypeInfo();
        }

        private static string GetKey(ModuleBuilder module, Type[] constantTypes, Type resultType, Type[] parameterTypes)
        {
            // TODO
            var key = resultType == typeof(void)
                          ? string.Format("{0}_{1}_ActionInvoker_{2}_{3}", module.Assembly.FullName, module.Name, constantTypes.Length, parameterTypes.Length)
                          : string.Format("{0}_{1}_FuncInvoker_{2}_{3}", module.Assembly.FullName, module.Name, constantTypes.Length, parameterTypes.Length);
            if (!Extensions.IsMono)
                return key;
            var stringBuilder = new StringBuilder(key);
            foreach (var type in constantTypes)
            {
                stringBuilder.Append('_');
                stringBuilder.Append(type.FullName);
            }
            foreach (var type in parameterTypes)
            {
                stringBuilder.Append('_');
                stringBuilder.Append(type.FullName);
            }
            stringBuilder.Append('_');
            stringBuilder.Append(resultType.FullName);
            return stringBuilder.ToString();
        }

        private static Func<DynamicMethod, IntPtr> EmitDynamicMethodPointerExtractor()
        {
            if (Extensions.IsMono)
            {
                return dynMethod =>
                    {
                        var handle = dynMethod.MethodHandle;
                        RuntimeHelpers.PrepareMethod(handle);
                        return handle.GetFunctionPointer();
                    };
            }
            var method = new DynamicMethod("DynamicMethodPointerExtractor", typeof(IntPtr), new[] {typeof(DynamicMethod)}, typeof(LambdaExpressionEmitter).Module, true);
            using (var il = new GroboIL(method))
            {
                il.Ldarg(0); // stack: [dynamicMethod]
                var getMethodDescriptorMethod = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);
                if (getMethodDescriptorMethod == null)
                    throw new MissingMethodException(typeof(DynamicMethod).Name, "GetMethodDescriptor");
                il.Call(getMethodDescriptorMethod); // stack: [dynamicMethod.GetMethodDescriptor()]
                var runtimeMethodHandle = il.DeclareLocal(typeof(RuntimeMethodHandle));
                il.Stloc(runtimeMethodHandle); // runtimeMethodHandle = dynamicMethod.GetMethodDescriptor(); stack: []
                il.Ldloc(runtimeMethodHandle); // stack: [runtimeMethodHandle]
                var prepareMethodMethod = typeof(RuntimeHelpers).GetMethod("PrepareMethod", new[] {typeof(RuntimeMethodHandle)});
                if (prepareMethodMethod == null)
                    throw new MissingMethodException(typeof(RuntimeHelpers).Name, "PrepareMethod");
                il.Call(prepareMethodMethod); // RuntimeHelpers.PrepareMethod(runtimeMethodHandle)
                var getFunctionPointerMethod = typeof(RuntimeMethodHandle).GetMethod("GetFunctionPointer", BindingFlags.Instance | BindingFlags.Public);
                if (getFunctionPointerMethod == null)
                    throw new MissingMethodException(typeof(RuntimeMethodHandle).Name, "GetFunctionPointer");
                il.Ldloca(runtimeMethodHandle); // stack: [&runtimeMethodHandle]
                il.Call(getFunctionPointerMethod); // stack: [runtimeMethodHandle.GetFunctionPointer()]
                il.Ret(); // return runtimeMethodHandle.GetFunctionPointer()
            }
            return (Func<DynamicMethod, IntPtr>)method.CreateDelegate(typeof(Func<DynamicMethod, IntPtr>));
        }

        public static readonly Func<DynamicMethod, IntPtr> DynamicMethodPointerExtractor = EmitDynamicMethodPointerExtractor();

        private static readonly MethodInfo gcKeepAliveMethod = ((MethodCallExpression)((Expression<Action>)(() => GC.KeepAlive(null))).Body).Method;

        private static readonly Hashtable types = new Hashtable();
        private static readonly object typesLock = new object();
    }

    public class MonoSucks
    {
        public static Type Get(Type[] constantTypes, Type resultType, Type[] parameterTypes)
        {
            bool isFunc = resultType != typeof(void);
            string typeName = string.Format("{0}Invoker_{1}_{2}`{3}", isFunc ? "Func" : "Action",
                                            constantTypes.Length, parameterTypes.Length, constantTypes.Length + parameterTypes.Length + (isFunc ? 1 : 0));
            var type = typeof(MonoSucks).GetNestedType(typeName);
            if (type == null)
                throw new NotSupportedException("TODO");
            return type;
        }

        public class CompiledLambdaTarget
        {
            public CompiledLambdaTarget(object target, Delegate compiledLambda)
            {
                this.target = target;
                this.compiledLambda = compiledLambda;
            }

            public object target;
            public Delegate compiledLambda;
        }

        public class FuncInvoker_0_0<TResult>
        {
            public FuncInvoker_0_0(Func<TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke()
            {
                return func();
            }

            private readonly Func<TResult> func;
        }

        public class FuncInvoker_1_0<TConst1, TResult>
        {
            public FuncInvoker_1_0(TConst1 const1, Func<TConst1, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TResult> func;
        }

        public class FuncInvoker_0_1<TParam1, TResult>
        {
            public FuncInvoker_0_1(Func<TParam1, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(param1);
            }

            private readonly Func<TParam1, TResult> func;
        }

        public class FuncInvoker_2_0<TConst1, TConst2, TResult>
        {
            public FuncInvoker_2_0(TConst1 const1, TConst2 const2, Func<TConst1, TConst2, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1, const2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Func<TConst1, TConst2, TResult> func;
        }

        public class FuncInvoker_1_1<TConst1, TParam1, TResult>
        {
            public FuncInvoker_1_1(TConst1 const1, Func<TConst1, TParam1, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(const1, param1);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TParam1, TResult> func;
        }

        public class FuncInvoker_0_2<TParam1, TParam2, TResult>
        {
            public FuncInvoker_0_2(Func<TParam1, TParam2, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2)
            {
                return func(param1, param2);
            }

            private readonly Func<TParam1, TParam2, TResult> func;
        }

        public class FuncInvoker_3_0<TConst1, TConst2, TConst3, TResult>
        {
            public FuncInvoker_3_0(TConst1 const1, TConst2 const2, TConst3 const3, Func<TConst1, TConst2, TConst3, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1, const2, const3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Func<TConst1, TConst2, TConst3, TResult> func;
        }

        public class FuncInvoker_2_1<TConst1, TConst2, TParam1, TResult>
        {
            public FuncInvoker_2_1(TConst1 const1, TConst2 const2, Func<TConst1, TConst2, TParam1, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(const1, const2, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Func<TConst1, TConst2, TParam1, TResult> func;
        }

        public class FuncInvoker_1_2<TConst1, TParam1, TParam2, TResult>
        {
            public FuncInvoker_1_2(TConst1 const1, Func<TConst1, TParam1, TParam2, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2)
            {
                return func(const1, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TParam1, TParam2, TResult> func;
        }

        public class FuncInvoker_0_3<TParam1, TParam2, TParam3, TResult>
        {
            public FuncInvoker_0_3(Func<TParam1, TParam2, TParam3, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                return func(param1, param2, param3);
            }

            private readonly Func<TParam1, TParam2, TParam3, TResult> func;
        }

        public class FuncInvoker_4_0<TConst1, TConst2, TConst3, TConst4, TResult>
        {
            public FuncInvoker_4_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Func<TConst1, TConst2, TConst3, TConst4, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1, const2, const3, const4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TResult> func;
        }

        public class FuncInvoker_3_1<TConst1, TConst2, TConst3, TParam1, TResult>
        {
            public FuncInvoker_3_1(TConst1 const1, TConst2 const2, TConst3 const3, Func<TConst1, TConst2, TConst3, TParam1, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(const1, const2, const3, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Func<TConst1, TConst2, TConst3, TParam1, TResult> func;
        }

        public class FuncInvoker_2_2<TConst1, TConst2, TParam1, TParam2, TResult>
        {
            public FuncInvoker_2_2(TConst1 const1, TConst2 const2, Func<TConst1, TConst2, TParam1, TParam2, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2)
            {
                return func(const1, const2, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Func<TConst1, TConst2, TParam1, TParam2, TResult> func;
        }

        public class FuncInvoker_1_3<TConst1, TParam1, TParam2, TParam3, TResult>
        {
            public FuncInvoker_1_3(TConst1 const1, Func<TConst1, TParam1, TParam2, TParam3, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                return func(const1, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TParam1, TParam2, TParam3, TResult> func;
        }

        public class FuncInvoker_0_4<TParam1, TParam2, TParam3, TParam4, TResult>
        {
            public FuncInvoker_0_4(Func<TParam1, TParam2, TParam3, TParam4, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                return func(param1, param2, param3, param4);
            }

            private readonly Func<TParam1, TParam2, TParam3, TParam4, TResult> func;
        }

        public class FuncInvoker_5_0<TConst1, TConst2, TConst3, TConst4, TConst5, TResult>
        {
            public FuncInvoker_5_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1, const2, const3, const4, const5);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TResult> func;
        }

        public class FuncInvoker_4_1<TConst1, TConst2, TConst3, TConst4, TParam1, TResult>
        {
            public FuncInvoker_4_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Func<TConst1, TConst2, TConst3, TConst4, TParam1, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(const1, const2, const3, const4, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TParam1, TResult> func;
        }

        public class FuncInvoker_3_2<TConst1, TConst2, TConst3, TParam1, TParam2, TResult>
        {
            public FuncInvoker_3_2(TConst1 const1, TConst2 const2, TConst3 const3, Func<TConst1, TConst2, TConst3, TParam1, TParam2, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2)
            {
                return func(const1, const2, const3, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Func<TConst1, TConst2, TConst3, TParam1, TParam2, TResult> func;
        }

        public class FuncInvoker_2_3<TConst1, TConst2, TParam1, TParam2, TParam3, TResult>
        {
            public FuncInvoker_2_3(TConst1 const1, TConst2 const2, Func<TConst1, TConst2, TParam1, TParam2, TParam3, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                return func(const1, const2, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Func<TConst1, TConst2, TParam1, TParam2, TParam3, TResult> func;
        }

        public class FuncInvoker_1_4<TConst1, TParam1, TParam2, TParam3, TParam4, TResult>
        {
            public FuncInvoker_1_4(TConst1 const1, Func<TConst1, TParam1, TParam2, TParam3, TParam4, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                return func(const1, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TParam1, TParam2, TParam3, TParam4, TResult> func;
        }

        public class FuncInvoker_0_5<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>
        {
            public FuncInvoker_0_5(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                return func(param1, param2, param3, param4, param5);
            }

            private readonly Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func;
        }

        public class FuncInvoker_6_0<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TResult>
        {
            public FuncInvoker_6_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1, const2, const3, const4, const5, const6);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TResult> func;
        }

        public class FuncInvoker_5_1<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TResult>
        {
            public FuncInvoker_5_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(const1, const2, const3, const4, const5, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TResult> func;
        }

        public class FuncInvoker_4_2<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TResult>
        {
            public FuncInvoker_4_2(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Func<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2)
            {
                return func(const1, const2, const3, const4, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TResult> func;
        }

        public class FuncInvoker_3_3<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TResult>
        {
            public FuncInvoker_3_3(TConst1 const1, TConst2 const2, TConst3 const3, Func<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                return func(const1, const2, const3, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Func<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TResult> func;
        }

        public class FuncInvoker_2_4<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TResult>
        {
            public FuncInvoker_2_4(TConst1 const1, TConst2 const2, Func<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                return func(const1, const2, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Func<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TResult> func;
        }

        public class FuncInvoker_1_5<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TResult>
        {
            public FuncInvoker_1_5(TConst1 const1, Func<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                return func(const1, param1, param2, param3, param4, param5);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func;
        }

        public class FuncInvoker_0_6<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>
        {
            public FuncInvoker_0_6(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
            {
                return func(param1, param2, param3, param4, param5, param6);
            }

            private readonly Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> func;
        }

        public class FuncInvoker_7_0<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TResult>
        {
            public FuncInvoker_7_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1, const2, const3, const4, const5, const6, const7);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TResult> func;
        }

        public class FuncInvoker_6_1<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TResult>
        {
            public FuncInvoker_6_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(const1, const2, const3, const4, const5, const6, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TResult> func;
        }

        public class FuncInvoker_5_2<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TResult>
        {
            public FuncInvoker_5_2(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2)
            {
                return func(const1, const2, const3, const4, const5, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TResult> func;
        }

        public class FuncInvoker_4_3<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TResult>
        {
            public FuncInvoker_4_3(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Func<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                return func(const1, const2, const3, const4, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TResult> func;
        }

        public class FuncInvoker_3_4<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TResult>
        {
            public FuncInvoker_3_4(TConst1 const1, TConst2 const2, TConst3 const3, Func<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                return func(const1, const2, const3, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Func<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TResult> func;
        }

        public class FuncInvoker_2_5<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TResult>
        {
            public FuncInvoker_2_5(TConst1 const1, TConst2 const2, Func<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                return func(const1, const2, param1, param2, param3, param4, param5);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Func<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func;
        }

        public class FuncInvoker_1_6<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>
        {
            public FuncInvoker_1_6(TConst1 const1, Func<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
            {
                return func(const1, param1, param2, param3, param4, param5, param6);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> func;
        }

        public class FuncInvoker_0_7<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>
        {
            public FuncInvoker_0_7(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
            {
                return func(param1, param2, param3, param4, param5, param6, param7);
            }

            private readonly Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult> func;
        }

        public class FuncInvoker_8_0<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TResult>
        {
            public FuncInvoker_8_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, TConst8 const8, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.const8 = const8;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1, const2, const3, const4, const5, const6, const7, const8);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly TConst8 const8;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TResult> func;
        }

        public class FuncInvoker_7_1<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TResult>
        {
            public FuncInvoker_7_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(const1, const2, const3, const4, const5, const6, const7, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TResult> func;
        }

        public class FuncInvoker_6_2<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TResult>
        {
            public FuncInvoker_6_2(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2)
            {
                return func(const1, const2, const3, const4, const5, const6, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TResult> func;
        }

        public class FuncInvoker_5_3<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TResult>
        {
            public FuncInvoker_5_3(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                return func(const1, const2, const3, const4, const5, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TResult> func;
        }

        public class FuncInvoker_4_4<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TResult>
        {
            public FuncInvoker_4_4(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Func<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                return func(const1, const2, const3, const4, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TResult> func;
        }

        public class FuncInvoker_3_5<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TResult>
        {
            public FuncInvoker_3_5(TConst1 const1, TConst2 const2, TConst3 const3, Func<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                return func(const1, const2, const3, param1, param2, param3, param4, param5);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Func<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func;
        }

        public class FuncInvoker_2_6<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>
        {
            public FuncInvoker_2_6(TConst1 const1, TConst2 const2, Func<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
            {
                return func(const1, const2, param1, param2, param3, param4, param5, param6);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Func<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> func;
        }

        public class FuncInvoker_1_7<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>
        {
            public FuncInvoker_1_7(TConst1 const1, Func<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
            {
                return func(const1, param1, param2, param3, param4, param5, param6, param7);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult> func;
        }

        public class FuncInvoker_0_8<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult>
        {
            public FuncInvoker_0_8(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8)
            {
                return func(param1, param2, param3, param4, param5, param6, param7, param8);
            }

            private readonly Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult> func;
        }

        public class FuncInvoker_9_0<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TConst9, TResult>
        {
            public FuncInvoker_9_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, TConst8 const8, TConst9 const9, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TConst9, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.const8 = const8;
                this.const9 = const9;
                this.func = func;
            }

            public TResult Invoke()
            {
                return func(const1, const2, const3, const4, const5, const6, const7, const8, const9);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly TConst8 const8;
            private readonly TConst9 const9;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TConst9, TResult> func;
        }

        public class FuncInvoker_8_1<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TParam1, TResult>
        {
            public FuncInvoker_8_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, TConst8 const8, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TParam1, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.const8 = const8;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1)
            {
                return func(const1, const2, const3, const4, const5, const6, const7, const8, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly TConst8 const8;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TParam1, TResult> func;
        }

        public class FuncInvoker_7_2<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TParam2, TResult>
        {
            public FuncInvoker_7_2(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TParam2, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2)
            {
                return func(const1, const2, const3, const4, const5, const6, const7, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TParam2, TResult> func;
        }

        public class FuncInvoker_6_3<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TParam3, TResult>
        {
            public FuncInvoker_6_3(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TParam3, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                return func(const1, const2, const3, const4, const5, const6, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TParam3, TResult> func;
        }

        public class FuncInvoker_5_4<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TParam4, TResult>
        {
            public FuncInvoker_5_4(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Func<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TParam4, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                return func(const1, const2, const3, const4, const5, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TParam4, TResult> func;
        }

        public class FuncInvoker_4_5<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TParam5, TResult>
        {
            public FuncInvoker_4_5(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Func<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                return func(const1, const2, const3, const4, param1, param2, param3, param4, param5);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Func<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TParam5, TResult> func;
        }

        public class FuncInvoker_3_6<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>
        {
            public FuncInvoker_3_6(TConst1 const1, TConst2 const2, TConst3 const3, Func<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
            {
                return func(const1, const2, const3, param1, param2, param3, param4, param5, param6);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Func<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult> func;
        }

        public class FuncInvoker_2_7<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>
        {
            public FuncInvoker_2_7(TConst1 const1, TConst2 const2, Func<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult> func)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
            {
                return func(const1, const2, param1, param2, param3, param4, param5, param6, param7);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Func<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult> func;
        }

        public class FuncInvoker_1_8<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult>
        {
            public FuncInvoker_1_8(TConst1 const1, Func<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult> func)
            {
                this.const1 = const1;
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8)
            {
                return func(const1, param1, param2, param3, param4, param5, param6, param7, param8);
            }

            private readonly TConst1 const1;
            private readonly Func<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult> func;
        }

        public class FuncInvoker_0_9<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TResult>
        {
            public FuncInvoker_0_9(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TResult> func)
            {
                this.func = func;
            }

            public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9)
            {
                return func(param1, param2, param3, param4, param5, param6, param7, param8, param9);
            }

            private readonly Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TResult> func;
        }

        public class ActionInvoker_0_0
        {
            public ActionInvoker_0_0(Action action)
            {
                this.action = action;
            }

            public void Invoke()
            {
                action();
            }

            private readonly Action action;
        }

        public class ActionInvoker_1_0<TConst1>
        {
            public ActionInvoker_1_0(TConst1 const1, Action<TConst1> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1> action;
        }

        public class ActionInvoker_0_1<TParam1>
        {
            public ActionInvoker_0_1(Action<TParam1> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(param1);
            }

            private readonly Action<TParam1> action;
        }

        public class ActionInvoker_2_0<TConst1, TConst2>
        {
            public ActionInvoker_2_0(TConst1 const1, TConst2 const2, Action<TConst1, TConst2> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1, const2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Action<TConst1, TConst2> action;
        }

        public class ActionInvoker_1_1<TConst1, TParam1>
        {
            public ActionInvoker_1_1(TConst1 const1, Action<TConst1, TParam1> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(const1, param1);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1, TParam1> action;
        }

        public class ActionInvoker_0_2<TParam1, TParam2>
        {
            public ActionInvoker_0_2(Action<TParam1, TParam2> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2)
            {
                action(param1, param2);
            }

            private readonly Action<TParam1, TParam2> action;
        }

        public class ActionInvoker_3_0<TConst1, TConst2, TConst3>
        {
            public ActionInvoker_3_0(TConst1 const1, TConst2 const2, TConst3 const3, Action<TConst1, TConst2, TConst3> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1, const2, const3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Action<TConst1, TConst2, TConst3> action;
        }

        public class ActionInvoker_2_1<TConst1, TConst2, TParam1>
        {
            public ActionInvoker_2_1(TConst1 const1, TConst2 const2, Action<TConst1, TConst2, TParam1> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(const1, const2, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Action<TConst1, TConst2, TParam1> action;
        }

        public class ActionInvoker_1_2<TConst1, TParam1, TParam2>
        {
            public ActionInvoker_1_2(TConst1 const1, Action<TConst1, TParam1, TParam2> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2)
            {
                action(const1, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1, TParam1, TParam2> action;
        }

        public class ActionInvoker_0_3<TParam1, TParam2, TParam3>
        {
            public ActionInvoker_0_3(Action<TParam1, TParam2, TParam3> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                action(param1, param2, param3);
            }

            private readonly Action<TParam1, TParam2, TParam3> action;
        }

        public class ActionInvoker_4_0<TConst1, TConst2, TConst3, TConst4>
        {
            public ActionInvoker_4_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Action<TConst1, TConst2, TConst3, TConst4> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1, const2, const3, const4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Action<TConst1, TConst2, TConst3, TConst4> action;
        }

        public class ActionInvoker_3_1<TConst1, TConst2, TConst3, TParam1>
        {
            public ActionInvoker_3_1(TConst1 const1, TConst2 const2, TConst3 const3, Action<TConst1, TConst2, TConst3, TParam1> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(const1, const2, const3, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Action<TConst1, TConst2, TConst3, TParam1> action;
        }

        public class ActionInvoker_2_2<TConst1, TConst2, TParam1, TParam2>
        {
            public ActionInvoker_2_2(TConst1 const1, TConst2 const2, Action<TConst1, TConst2, TParam1, TParam2> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2)
            {
                action(const1, const2, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Action<TConst1, TConst2, TParam1, TParam2> action;
        }

        public class ActionInvoker_1_3<TConst1, TParam1, TParam2, TParam3>
        {
            public ActionInvoker_1_3(TConst1 const1, Action<TConst1, TParam1, TParam2, TParam3> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                action(const1, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1, TParam1, TParam2, TParam3> action;
        }

        public class ActionInvoker_0_4<TParam1, TParam2, TParam3, TParam4>
        {
            public ActionInvoker_0_4(Action<TParam1, TParam2, TParam3, TParam4> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                action(param1, param2, param3, param4);
            }

            private readonly Action<TParam1, TParam2, TParam3, TParam4> action;
        }

        public class ActionInvoker_5_0<TConst1, TConst2, TConst3, TConst4, TConst5>
        {
            public ActionInvoker_5_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Action<TConst1, TConst2, TConst3, TConst4, TConst5> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1, const2, const3, const4, const5);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5> action;
        }

        public class ActionInvoker_4_1<TConst1, TConst2, TConst3, TConst4, TParam1>
        {
            public ActionInvoker_4_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Action<TConst1, TConst2, TConst3, TConst4, TParam1> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(const1, const2, const3, const4, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TParam1> action;
        }

        public class ActionInvoker_3_2<TConst1, TConst2, TConst3, TParam1, TParam2>
        {
            public ActionInvoker_3_2(TConst1 const1, TConst2 const2, TConst3 const3, Action<TConst1, TConst2, TConst3, TParam1, TParam2> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2)
            {
                action(const1, const2, const3, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Action<TConst1, TConst2, TConst3, TParam1, TParam2> action;
        }

        public class ActionInvoker_2_3<TConst1, TConst2, TParam1, TParam2, TParam3>
        {
            public ActionInvoker_2_3(TConst1 const1, TConst2 const2, Action<TConst1, TConst2, TParam1, TParam2, TParam3> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                action(const1, const2, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Action<TConst1, TConst2, TParam1, TParam2, TParam3> action;
        }

        public class ActionInvoker_1_4<TConst1, TParam1, TParam2, TParam3, TParam4>
        {
            public ActionInvoker_1_4(TConst1 const1, Action<TConst1, TParam1, TParam2, TParam3, TParam4> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                action(const1, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1, TParam1, TParam2, TParam3, TParam4> action;
        }

        public class ActionInvoker_0_5<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            public ActionInvoker_0_5(Action<TParam1, TParam2, TParam3, TParam4, TParam5> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                action(param1, param2, param3, param4, param5);
            }

            private readonly Action<TParam1, TParam2, TParam3, TParam4, TParam5> action;
        }

        public class ActionInvoker_6_0<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6>
        {
            public ActionInvoker_6_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1, const2, const3, const4, const5, const6);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6> action;
        }

        public class ActionInvoker_5_1<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1>
        {
            public ActionInvoker_5_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(const1, const2, const3, const4, const5, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1> action;
        }

        public class ActionInvoker_4_2<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2>
        {
            public ActionInvoker_4_2(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Action<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2)
            {
                action(const1, const2, const3, const4, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2> action;
        }

        public class ActionInvoker_3_3<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3>
        {
            public ActionInvoker_3_3(TConst1 const1, TConst2 const2, TConst3 const3, Action<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                action(const1, const2, const3, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Action<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3> action;
        }

        public class ActionInvoker_2_4<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4>
        {
            public ActionInvoker_2_4(TConst1 const1, TConst2 const2, Action<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                action(const1, const2, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Action<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4> action;
        }

        public class ActionInvoker_1_5<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            public ActionInvoker_1_5(TConst1 const1, Action<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                action(const1, param1, param2, param3, param4, param5);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5> action;
        }

        public class ActionInvoker_0_6<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            public ActionInvoker_0_6(Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
            {
                action(param1, param2, param3, param4, param5, param6);
            }

            private readonly Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action;
        }

        public class ActionInvoker_7_0<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7>
        {
            public ActionInvoker_7_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1, const2, const3, const4, const5, const6, const7);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7> action;
        }

        public class ActionInvoker_6_1<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1>
        {
            public ActionInvoker_6_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(const1, const2, const3, const4, const5, const6, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1> action;
        }

        public class ActionInvoker_5_2<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2>
        {
            public ActionInvoker_5_2(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2)
            {
                action(const1, const2, const3, const4, const5, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2> action;
        }

        public class ActionInvoker_4_3<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3>
        {
            public ActionInvoker_4_3(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Action<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                action(const1, const2, const3, const4, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3> action;
        }

        public class ActionInvoker_3_4<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4>
        {
            public ActionInvoker_3_4(TConst1 const1, TConst2 const2, TConst3 const3, Action<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                action(const1, const2, const3, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Action<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4> action;
        }

        public class ActionInvoker_2_5<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            public ActionInvoker_2_5(TConst1 const1, TConst2 const2, Action<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                action(const1, const2, param1, param2, param3, param4, param5);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Action<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5> action;
        }

        public class ActionInvoker_1_6<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            public ActionInvoker_1_6(TConst1 const1, Action<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
            {
                action(const1, param1, param2, param3, param4, param5, param6);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action;
        }

        public class ActionInvoker_0_7<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
        {
            public ActionInvoker_0_7(Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
            {
                action(param1, param2, param3, param4, param5, param6, param7);
            }

            private readonly Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> action;
        }

        public class ActionInvoker_8_0<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8>
        {
            public ActionInvoker_8_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, TConst8 const8, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.const8 = const8;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1, const2, const3, const4, const5, const6, const7, const8);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly TConst8 const8;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8> action;
        }

        public class ActionInvoker_7_1<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1>
        {
            public ActionInvoker_7_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(const1, const2, const3, const4, const5, const6, const7, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1> action;
        }

        public class ActionInvoker_6_2<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2>
        {
            public ActionInvoker_6_2(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2)
            {
                action(const1, const2, const3, const4, const5, const6, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2> action;
        }

        public class ActionInvoker_5_3<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3>
        {
            public ActionInvoker_5_3(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                action(const1, const2, const3, const4, const5, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3> action;
        }

        public class ActionInvoker_4_4<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4>
        {
            public ActionInvoker_4_4(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Action<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                action(const1, const2, const3, const4, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4> action;
        }

        public class ActionInvoker_3_5<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            public ActionInvoker_3_5(TConst1 const1, TConst2 const2, TConst3 const3, Action<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                action(const1, const2, const3, param1, param2, param3, param4, param5);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Action<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5> action;
        }

        public class ActionInvoker_2_6<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            public ActionInvoker_2_6(TConst1 const1, TConst2 const2, Action<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
            {
                action(const1, const2, param1, param2, param3, param4, param5, param6);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Action<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action;
        }

        public class ActionInvoker_1_7<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
        {
            public ActionInvoker_1_7(TConst1 const1, Action<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
            {
                action(const1, param1, param2, param3, param4, param5, param6, param7);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> action;
        }

        public class ActionInvoker_0_8<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>
        {
            public ActionInvoker_0_8(Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8);
            }

            private readonly Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8> action;
        }

        public class ActionInvoker_9_0<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TConst9>
        {
            public ActionInvoker_9_0(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, TConst8 const8, TConst9 const9, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TConst9> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.const8 = const8;
                this.const9 = const9;
                this.action = action;
            }

            public void Invoke()
            {
                action(const1, const2, const3, const4, const5, const6, const7, const8, const9);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly TConst8 const8;
            private readonly TConst9 const9;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TConst9> action;
        }

        public class ActionInvoker_8_1<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TParam1>
        {
            public ActionInvoker_8_1(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, TConst8 const8, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TParam1> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.const8 = const8;
                this.action = action;
            }

            public void Invoke(TParam1 param1)
            {
                action(const1, const2, const3, const4, const5, const6, const7, const8, param1);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly TConst8 const8;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TConst8, TParam1> action;
        }

        public class ActionInvoker_7_2<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TParam2>
        {
            public ActionInvoker_7_2(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, TConst7 const7, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TParam2> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.const7 = const7;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2)
            {
                action(const1, const2, const3, const4, const5, const6, const7, param1, param2);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly TConst7 const7;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TConst7, TParam1, TParam2> action;
        }

        public class ActionInvoker_6_3<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TParam3>
        {
            public ActionInvoker_6_3(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, TConst6 const6, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TParam3> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.const6 = const6;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                action(const1, const2, const3, const4, const5, const6, param1, param2, param3);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly TConst6 const6;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TConst6, TParam1, TParam2, TParam3> action;
        }

        public class ActionInvoker_5_4<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TParam4>
        {
            public ActionInvoker_5_4(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, TConst5 const5, Action<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TParam4> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.const5 = const5;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                action(const1, const2, const3, const4, const5, param1, param2, param3, param4);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly TConst5 const5;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TConst5, TParam1, TParam2, TParam3, TParam4> action;
        }

        public class ActionInvoker_4_5<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            public ActionInvoker_4_5(TConst1 const1, TConst2 const2, TConst3 const3, TConst4 const4, Action<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TParam5> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.const4 = const4;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
            {
                action(const1, const2, const3, const4, param1, param2, param3, param4, param5);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly TConst4 const4;
            private readonly Action<TConst1, TConst2, TConst3, TConst4, TParam1, TParam2, TParam3, TParam4, TParam5> action;
        }

        public class ActionInvoker_3_6<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            public ActionInvoker_3_6(TConst1 const1, TConst2 const2, TConst3 const3, Action<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.const3 = const3;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
            {
                action(const1, const2, const3, param1, param2, param3, param4, param5, param6);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly TConst3 const3;
            private readonly Action<TConst1, TConst2, TConst3, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action;
        }

        public class ActionInvoker_2_7<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
        {
            public ActionInvoker_2_7(TConst1 const1, TConst2 const2, Action<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> action)
            {
                this.const1 = const1;
                this.const2 = const2;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
            {
                action(const1, const2, param1, param2, param3, param4, param5, param6, param7);
            }

            private readonly TConst1 const1;
            private readonly TConst2 const2;
            private readonly Action<TConst1, TConst2, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> action;
        }

        public class ActionInvoker_1_8<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>
        {
            public ActionInvoker_1_8(TConst1 const1, Action<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8> action)
            {
                this.const1 = const1;
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8)
            {
                action(const1, param1, param2, param3, param4, param5, param6, param7, param8);
            }

            private readonly TConst1 const1;
            private readonly Action<TConst1, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8> action;
        }

        public class ActionInvoker_0_9<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9>
        {
            public ActionInvoker_0_9(Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9> action)
            {
                this.action = action;
            }

            public void Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9);
            }

            private readonly Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9> action;
        }
    }
}