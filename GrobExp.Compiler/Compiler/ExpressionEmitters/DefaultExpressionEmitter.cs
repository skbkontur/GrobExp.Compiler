using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class DefaultExpressionEmitter : ExpressionEmitter<DefaultExpression>
    {
        protected override bool EmitInternal(DefaultExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            resultType = node.Type;
            if(node.Type != typeof(void))
                context.EmitLoadDefaultValue(node.Type);
            return false;
        }
    }
}