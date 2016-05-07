using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GrobExp.Compiler
{
    internal class ClosureSubstituter : ExpressionVisitor
    {
        public ClosureSubstituter(ParameterExpression closure, Dictionary<ParameterExpression, FieldInfo> parameters)
        {
            this.closure = closure;
            this.parameters = parameters;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            FieldInfo field;
            return parameters.TryGetValue(node, out field)
                       ? node.Type == field.FieldType
                             ? Expression.MakeMemberAccess(closure, field)
                             : Expression.MakeMemberAccess(Expression.MakeMemberAccess(closure, field), field.FieldType.GetField("Value", BindingFlags.Public | BindingFlags.Instance))
                       : base.VisitParameter(node);
        }

        private readonly ParameterExpression closure;
        private readonly Dictionary<ParameterExpression, FieldInfo> parameters;
    }
}