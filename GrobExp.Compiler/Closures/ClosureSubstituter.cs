using System.Collections.Generic;
using System.Linq.Expressions;

namespace GrobExp.Compiler.Closures
{
    internal class ClosureSubstituter : ExpressionVisitor
    {
        public ClosureSubstituter(ParameterExpression closure, IClosureBuilder closureBuilder, Dictionary<ParameterExpression, int> parameters)
        {
            this.closure = closure;
            this.closureBuilder = closureBuilder;
            this.parameters = parameters;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            int field;
            return parameters.TryGetValue(node, out field)
                       ? closureBuilder.MakeAccess(closure, field)
                       : base.VisitParameter(node);
        }

        private readonly ParameterExpression closure;
        private readonly IClosureBuilder closureBuilder;
        private readonly Dictionary<ParameterExpression, int> parameters;
    }
}