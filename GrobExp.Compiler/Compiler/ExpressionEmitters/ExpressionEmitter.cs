using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal abstract class ExpressionEmitter<TExpression> : IExpressionEmitter where TExpression : Expression
    {
        public bool Emit(Expression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            return EmitInternal((TExpression)node, context, returnDefaultValueLabel, node.Type == typeof(void) ? ResultType.Void : whatReturn, extend, out resultType);
        }

        protected abstract bool EmitInternal(TExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType);
    }
}