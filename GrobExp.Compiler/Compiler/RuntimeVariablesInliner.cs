using System.Linq;
using System.Linq.Expressions;

using GrobExp.Compiler.ExpressionEmitters;

namespace GrobExp.Compiler
{
    internal class RuntimeVariablesInliner : ExpressionVisitor
    {
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            var constructor = typeof(RuntimeVariables).GetConstructor(new[] { typeof(object[]) });
            return Expression.New(constructor, Expression.NewArrayInit(typeof(object), node.Variables.Select(parameter => parameter.Type.IsValueType ? Expression.Convert(parameter, typeof(object)) : (Expression)parameter)));
        }
    }
}