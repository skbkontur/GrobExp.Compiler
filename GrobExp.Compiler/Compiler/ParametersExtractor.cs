using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GrobExp.Compiler
{
    public class ParametersExtractor : ExpressionVisitor
    {
        public ParameterExpression[] Extract(Expression exp)
        {
            localParameters.Clear();
            result.Clear();
            Visit(exp);
            return result.ToArray();
        }

        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            foreach(var parameter in lambda.Parameters)
                localParameters.Add(parameter);
            var res = base.VisitLambda(lambda);
            foreach(var parameter in lambda.Parameters)
                localParameters.Remove(parameter);
            return res;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            foreach(var variable in node.Variables)
                localParameters.Add(variable);
            var res = base.VisitBlock(node);
            foreach(var variable in node.Variables)
                localParameters.Remove(variable);
            return res;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if(!localParameters.Contains(p))
                result.Add(p);
            return base.VisitParameter(p);
        }

        private readonly HashSet<ParameterExpression> localParameters = new HashSet<ParameterExpression>();
        private readonly HashSet<ParameterExpression> result = new HashSet<ParameterExpression>();
    }
}