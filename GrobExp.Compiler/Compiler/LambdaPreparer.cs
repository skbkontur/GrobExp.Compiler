using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GrobExp.Compiler
{
    internal class LambdaPreparer : ExpressionVisitor
    {
        protected override Expression VisitUnary(UnaryExpression node)
        {
            return node.NodeType == ExpressionType.Quote ? node : base.VisitUnary(node);
        }

        // TODO inline small lambdas
        //protected override Expression VisitInvocation(InvocationExpression node)
        //{
        //    if(node.Expression.NodeType != ExpressionType.Lambda)
        //        return base.VisitInvocation(node);
        //    var lambda = (LambdaExpression)node.Expression;
        //    var expressions = lambda.Parameters.Select((t, i) => Expression.Assign(t, Visit(node.Arguments[i]))).Cast<Expression>().ToList();
        //    expressions.Add(Visit(new LabelsCloner().Visit(lambda.Body)));
        //    return Expression.Block(lambda.Body.Type, lambda.Parameters, expressions);
        //}

        protected override Expression VisitExtension(Expression node)
        {
            return !(node is TypedDebugInfoExpression) && node.CanReduce ? Visit(node.Reduce()) : base.VisitExtension(node);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            var site = CallSite.Create(node.DelegateType, node.Binder);
            var siteType = site.GetType();
            var constant = Expression.Constant(site, siteType);
            return Expression.Call(Expression.MakeMemberAccess(constant, siteType.GetField("Target")), node.DelegateType.GetMethod("Invoke"), new[] {constant}.Concat(node.Arguments.Select(Visit)));
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if(node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);
                if(left.Type.IsNullable() && right.Type == typeof(object))
                    right = Expression.Convert(right, node.Left.Type);
                return node.Update(left, (LambdaExpression)Visit(node.Conversion), right);
            }
            return base.VisitBinary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if(node.Object == null)
                return base.VisitMethodCall(node);
            var indexer = node.Object.Type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
            if(indexer != null && indexer.GetGetMethod(true) == node.Method)
                return Expression.MakeIndex(node.Object, indexer, node.Arguments);
            return base.VisitMethodCall(node);
        }
    }
}