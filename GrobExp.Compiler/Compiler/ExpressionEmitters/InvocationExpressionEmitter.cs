using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class InvocationExpressionEmitter : ExpressionEmitter<InvocationExpression>
    {
        protected override bool EmitInternal(InvocationExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            bool result;
            if(node.Expression.NodeType != ExpressionType.Lambda)
            {
                Type delegateType;
                result = ExpressionEmittersCollection.Emit(node.Expression, context, returnDefaultValueLabel, ResultType.Value, extend, out delegateType);
                context.EmitLoadArguments(node.Arguments.ToArray());
                var invokeMethod = delegateType.GetMethod("Invoke", node.Arguments.Select(argument => argument.Type).ToArray());
                context.Il.Call(invokeMethod, delegateType);
            }
            else
            {
                result = false;
                var lambda = (LambdaExpression)node.Expression;
                Type[] constantTypes;
                var compiledLambda = LambdaExpressionEmitter.CompileAndLoadConstants(lambda, context, out constantTypes);
                context.EmitLoadArguments(node.Arguments.ToArray());
                context.LoadCompiledLambdaPointer(compiledLambda);
                context.Il.Calli(CallingConventions.Standard, lambda.ReturnType, constantTypes.Concat(lambda.Parameters.Select(parameter => parameter.Type)).ToArray());
            }
            resultType = node.Type;
            return result;
        }
    }
}