using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;

using GrEmit;
using GrEmit.Utils;

namespace GrobExp.Compiler
{
    internal class ExpressionPrivateMembersAccessor : ExpressionVisitor
    {
        private static bool IsNestedlyPublic(Type type)
        {
            if (type == null || type.IsGenericParameter) return true;

            if(type.IsArray)
            {
                if (!type.IsNested)
                {
                    if (!type.IsPublic)
                        return false;
                }
                else if (!type.IsNestedPublic || !IsNestedlyPublic(type.DeclaringType))
                    return false;
                var elem = type.GetElementType();
                return IsNestedlyPublic(elem);
            }

            if(type.IsGenericType)
            {
                if(!type.IsNested)
                {
                    if(!type.IsPublic)
                        return false;
                }
                else if(!type.IsNestedPublic || !IsNestedlyPublic(type.DeclaringType))
                    return false;
                var parameters = type.GetGenericArguments();
                return parameters.All(IsNestedlyPublic);
            }

            if (!type.IsNested)
                return type.IsPublic;
            return type.IsNestedPublic && IsNestedlyPublic(type.DeclaringType);
        }

        private static Expression GetGetter(FieldInfo field, Expression obj, Type type)
        {
            var extractor = FieldsExtractor.GetExtractor(field);
            return Expression.Convert(Expression.Invoke(Expression.Constant(extractor), obj ?? Expression.Constant(null)), type);
        }

        private static Expression GetSetter(FieldInfo field, Expression obj, Expression newValue)
        {
            var setter = FieldsExtractor.GetSetter(field);
            return Expression.Invoke(Expression.Constant(setter), obj ?? Expression.Constant(null), Expression.Convert(newValue, typeof(object)));
        }

        private static bool NeedsToBeReplacedByGetter(Expression node)
        {
            var access = node as MemberExpression;
            if (access == null)
                return false;
            return NeedsToBeReplacedByGetter(access);
        }

        private static bool NeedsToBeReplacedByGetter(MemberExpression node)
        {
            var member = node.Member;
            var expression = node.Expression;

            return (expression != null && !IsNestedlyPublic(expression.Type)) ||
                   (member is FieldInfo && !((FieldInfo)member).IsPublic) ||
                   (member is PropertyInfo && !((PropertyInfo)member).GetGetMethod(true).IsPublic);
        }

        private static Expression GetObjectFromGetter(Expression getter)
        {
            var convert = (UnaryExpression)getter;
            var invocation = (InvocationExpression)convert.Operand;
            var obj = invocation.Arguments[0];
            var asConst = obj as ConstantExpression;
            if(asConst != null && asConst.Value == null)
                return null;
            return obj;
        }

        private static Expression GetInvocation(MethodInfo method, Expression obj, Expression[] arguments)
        {
            if(obj != null)
                arguments = new[] {obj}.Concat(arguments).ToArray();

            foreach(var arg in arguments)
                if(!IsNestedlyPublic(arg.Type))
                {
                    throw new InvalidOperationException(string.Format(
                        "Non-public method '{0}' with argument or return value of non-public type '{1}' is not allowed!",
                        method.Name, Formatter.Format(arg.Type)));
                }

            var methodDelegate = MethodInvokerBuilder.GetInvoker(method);
            var methodDelegateType = Extensions.GetDelegateType(arguments.Select(a => a.Type).ToArray(), method.ReturnType);

            return Expression.Invoke(Expression.Convert(Expression.Constant(methodDelegate), methodDelegateType), arguments);
        }

        private void CheckTypePublicity(Type type)
        {
            if(!IsNestedlyPublic(type))
                throw new InvalidOperationException(string.Format("Non-public type '{0}' is not allowed!", Formatter.Format(type)));
        }

        protected override Expression VisitNew(NewExpression node)
        {
            CheckTypePublicity(node.Type);
            return base.VisitNew(node);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            CheckTypePublicity(node.Type);
            return base.VisitNewArray(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var member = node.Member;
            var expression = Visit(node.Expression);

            if (NeedsToBeReplacedByGetter(node))
            {
                CheckTypePublicity(node.Type);
                switch(node.Member.MemberType)
                {
                case MemberTypes.Property:
                    {
                        var getter = ((PropertyInfo)node.Member).GetGetMethod(true);
                        return GetInvocation(getter, expression, new Expression[0]);
                    }
                case MemberTypes.Field:
                    if (expression != null && expression.NodeType == ExpressionType.Convert)
                        expression = ((UnaryExpression)expression).Operand;
                    return GetGetter((FieldInfo)member, expression, node.Type);
                default:
                    throw new InvalidOperationException(
                        string.Format("Unknown MemberType '{0}'", node.Member.MemberType));
                }
            }

            return node.Update(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var newObject = Visit(node.Object);
            var arguments = node.Arguments;
            var newArguments = Visit(arguments).ToArray();
            var argumentsTypes = node.Method.GetParameters().Select(p => p.ParameterType).ToArray();

            var variables = new List<ParameterExpression>();
            var beforeInvocation = new List<Expression>();
            var afterInvocation = new List<Expression>();

            for(int i = 0; i < arguments.Count; i++)
            {
                if(NeedsToBeReplacedByGetter(arguments[i]) && argumentsTypes[i].IsByRef)
                {
                    var access = (MemberExpression)arguments[i];
                    var getter = newArguments[i];
                    var local = Expression.Parameter(argumentsTypes[i].GetElementType());
                    var setter = GetSetter((FieldInfo)access.Member, GetObjectFromGetter(getter), local);

                    variables.Add(local);
                    beforeInvocation.Add(Expression.Assign(local, getter));
                    afterInvocation.Add(setter);
                    newArguments[i] = local;
                }
            }

            Expression newInvocation;
            if(node.Method.IsPublic && IsNestedlyPublic(node.Method.DeclaringType))
                newInvocation = node.Update(newObject, newArguments);
            else
                newInvocation = GetInvocation(node.Method, newObject, newArguments);

            if(variables.Count > 0)
            {
                ParameterExpression returnVariable = null;
                if(newInvocation.Type != typeof(void))
                {
                    returnVariable = Expression.Parameter(newInvocation.Type);
                    variables.Add(returnVariable);
                    newInvocation = Expression.Assign(returnVariable, newInvocation);
                }

                var blockExpressions = new List<Expression>();
                blockExpressions.AddRange(beforeInvocation);
                blockExpressions.Add(newInvocation);
                blockExpressions.AddRange(afterInvocation);
                if(returnVariable != null)
                    blockExpressions.Add(returnVariable);

                return Expression.Block(variables, blockExpressions);
            }

            return newInvocation;
        }
    }
}