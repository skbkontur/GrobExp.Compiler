using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class LambdaExpressionEmitter : ExpressionEmitter<LambdaExpression>
    {
        public static CompiledLambda CompileAndLoadConstants(LambdaExpression node, EmittingContext context, out Type[] constantTypes)
        {
            var needClosure = context.ParsedLambda.ClosureParameter != null;
            var needConstants = context.ParsedLambda.ConstantsParameter != null;
            var constantParameters = new List<ParameterExpression>();
            if(needConstants)
                constantParameters.Add(context.ParsedLambda.ConstantsParameter);
            if(needClosure)
                constantParameters.Add(context.ParsedLambda.ClosureParameter);
            var parameters = constantParameters.Concat(node.Parameters).ToList();
            var lambda = Expression.Lambda(Extensions.GetDelegateType(parameters.Select(parameter => parameter.Type).ToArray(), node.ReturnType), node.Body, node.Name, node.TailCall, parameters);
            CompiledLambda compiledLambda;
            if(context.TypeBuilder == null)
                compiledLambda = LambdaCompiler.CompileInternal(lambda, context.DebugInfoGenerator, context.ParsedLambda, context.Options, context.CompiledLambdas);
            else
            {
                var method = context.TypeBuilder.DefineMethod(Guid.NewGuid().ToString(), MethodAttributes.Public | MethodAttributes.Static, lambda.ReturnType, lambda.Parameters.Select(parameter => parameter.Type).ToArray());
                for(var i = 0; i < parameters.Count; ++i)
                    method.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
                LambdaCompiler.CompileToMethodInternal(lambda, context.DebugInfoGenerator, context.ParsedLambda, context.Options, context.CompiledLambdas, method);
                compiledLambda = new CompiledLambda {Method = method};
            }
            context.CompiledLambdas.Add(compiledLambda);
            if(needConstants)
            {
                Type constantsType;
                ExpressionEmittersCollection.Emit(context.ParsedLambda.ConstantsParameter, context, out constantsType);
            }
            if(needClosure)
            {
                Type closureType;
                ExpressionEmittersCollection.Emit(context.ParsedLambda.ClosureParameter, context, out closureType);
            }
            constantTypes = constantParameters.Select(exp => exp.Type).ToArray();
            return compiledLambda;
        }

        protected override bool EmitInternal(LambdaExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var parameterTypes = node.Parameters.Select(parameter => parameter.Type).ToArray();

            Type[] constantTypes;
            var compiledLambda = CompileAndLoadConstants(node, context, out constantTypes);

            var il = context.Il;
            context.LoadCompiledLambdaPointer(compiledLambda);

            resultType = Extensions.GetDelegateType(parameterTypes, node.ReturnType);
            var types = constantTypes.Concat(new[] {typeof(IntPtr)}).ToArray();
            var module = (ModuleBuilder)(context.TypeBuilder == null ? null : context.TypeBuilder.Module);
            var subLambdaInvoker = DynamicMethodInvokerBuilder.BuildDynamicMethodInvoker(module, constantTypes, node.Body.Type, parameterTypes);
            il.Newobj(subLambdaInvoker.GetConstructor(types));
            il.Ldftn(subLambdaInvoker.GetMethod("Invoke"));
            il.Newobj(resultType.GetConstructor(new[] {typeof(object), typeof(IntPtr)}));
            return false;
        }
    }
}