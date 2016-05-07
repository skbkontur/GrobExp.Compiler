using System.Linq.Expressions;

namespace Compiler.Tests
{
    public class ParameterReplacer : ExpressionVisitor
    {
        public ParameterReplacer(ParameterExpression from, ParameterExpression to)
        {
            this.to = to;
            this.from = from;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == from ? to : base.VisitParameter(node);
        }

        private readonly ParameterExpression to;
        private readonly ParameterExpression from;
    }
}