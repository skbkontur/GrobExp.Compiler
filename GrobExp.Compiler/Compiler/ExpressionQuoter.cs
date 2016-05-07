using System;
using System.Linq.Expressions;

namespace GrobExp.Compiler
{
    public class ExpressionQuoter : ExpressionVisitor
    {
        public ExpressionQuoter(object closure)
        {
            closureType = closure.GetType();
            this.closure = Expression.Constant(closure, closureType);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node.Type == closureType ? closure : base.VisitParameter(node);
        }

        private readonly ConstantExpression closure;
        private readonly Type closureType;
    }
}