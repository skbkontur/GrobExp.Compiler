using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace GrobExp.Compiler
{
    internal class ExpressionAnonymousTypeReplacer : ExpressionVisitor
    {
        private readonly ModuleBuilder module;

        public ExpressionAnonymousTypeReplacer(ModuleBuilder module)
        {
            this.module = module;
        }

        private readonly Dictionary<Type, Type> typeCache = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> propertiesCache =
            new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        private readonly Dictionary<ParameterExpression, ParameterExpression> parameterCache =
            new Dictionary<ParameterExpression, ParameterExpression>();

        private bool IsAnonymousType(Type type)
        {
            return AnonymousTypeBuilder.IsAnonymousType(type);
        }

        private Type CreateAnonymousType(Type type)
        {
            if(!IsAnonymousType(type))
                return type;

            var properties = type.GetProperties();
            var newType = AnonymousTypeBuilder.CreateAnonymousType(properties
                .Select(p => p.PropertyType)
                .Select(CreateAnonymousType)
                .ToArray(),
                properties.Select(p => p.Name).ToArray(),
                module);
            var newProperties = newType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(property => property.Name);
            typeCache[type] = newType;
            propertiesCache[newType] = newProperties;
            return newType;
        }

        private Type GetTypeFromCache(Type type)
        {
            if(!typeCache.ContainsKey(type))
                return CreateAnonymousType(type);
            return typeCache[type];
        }

        private Type ReplaceType(Type type)
        {
            return IsAnonymousType(type) ? GetTypeFromCache(type) : type;
        }

        private Type ReplaceGenericType(Type type)
        {
            if ((!type.IsGenericType && !type.IsArray) || IsAnonymousType(type))
                return ReplaceType(type);

            if (type.IsArray)
            {
                var element = ReplaceGenericType(type.GetElementType());
                if(type.GetArrayRank() == 1)
                    return element.MakeArrayType();
                return element.MakeArrayType(type.GetArrayRank());
            }

            var pattern = type.GetGenericTypeDefinition();
            var generics = type.GetGenericArguments();
            generics = generics.Select(ReplaceGenericType).ToArray();
            return pattern.MakeGenericType(generics);
        }

        private bool ContainsAnonymousType(Type type)
        {
            if (IsAnonymousType(type))
                return true;
            if (type.IsArray)
                return ContainsAnonymousType(type.GetElementType());
            if (type.IsGenericType)
                return type.GetGenericArguments().Any(ContainsAnonymousType);
            return false;
        }

        private ParameterExpression GetParameterFromCache(ParameterExpression parameter)
        {
            if(!parameterCache.ContainsKey(parameter))
            {
                var newType = ReplaceGenericType(parameter.Type);
                var newParameter = Expression.Parameter(newType);
                parameterCache[parameter] = newParameter;
                return newParameter;
            }
            return parameterCache[parameter];
        }

        private ParameterExpression ReplaceParameter(ParameterExpression parameter)
        {
            return ContainsAnonymousType(parameter.Type) ? GetParameterFromCache(parameter) : parameter;
        }

        private ParameterExpression[] ReplaceParameters(IEnumerable<ParameterExpression> parameters)
        {
            return parameters.Select(ReplaceParameter).ToArray();
        }

        private MethodInfo ReplaceMethod(MethodInfo method)
        {
            if(!method.IsGenericMethod)
                return method;
            var pattern = method.GetGenericMethodDefinition();
            var generics = method.GetGenericArguments();
            generics = generics.Select(ReplaceGenericType).ToArray();
            return pattern.MakeGenericMethod(generics);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var expression = Visit(node.Expression);
            if (node.Expression != null && ContainsAnonymousType(node.Expression.Type))
            {
                var type = ReplaceGenericType(node.Expression.Type);
                var property = propertiesCache[type][node.Member.Name];
                return Expression.MakeMemberAccess(expression, property);
            }

            return node.Update(expression);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReplaceParameter(node);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var expressions = Visit(node.Expressions);
            var variables = ReplaceParameters(node.Variables);
            var type = ReplaceGenericType(node.Type);
            return Expression.Block(type, variables, expressions);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var body = Visit(node.Body);
            var parameters = ReplaceParameters(node.Parameters);
            var type = ReplaceGenericType(node.Type);
            return Expression.Lambda(type, body, node.Name, node.TailCall, parameters);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var arguments = Visit(node.Arguments);
            var type = ReplaceGenericType(node.Type);

            if (node.Constructor == null)
                return Expression.New(node.Type);

            var constructorTypes = node.Constructor.GetParameters().Select(
                p => ReplaceGenericType(p.ParameterType)).ToArray();

            MemberInfo[] members = null;
            if (node.Members != null)
                members = node.Members.ToArray();

            if(members != null && ContainsAnonymousType(node.Type))
            {
                var properties = propertiesCache[type];
                // ReSharper disable once CoVariantArrayConversion
                members = members.Select(m => properties[m.Name]).ToArray();
            }

            var constructor = type.GetConstructor(constructorTypes);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if(members == null)
                return Expression.New(constructor, arguments);
            return Expression.New(constructor, arguments, members);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            var expressions = Visit(node.Expressions);
            var type = ReplaceGenericType(node.Type.GetElementType());
            if(node.NodeType == ExpressionType.NewArrayBounds)
                return Expression.NewArrayBounds(type, expressions);
            return Expression.NewArrayInit(type, expressions);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = ReplaceMethod(node.Method);
            var arguments = Visit(node.Arguments);
            var obj = Visit(node.Object);
            return Expression.Call(obj, method, arguments);
        }
    }
}