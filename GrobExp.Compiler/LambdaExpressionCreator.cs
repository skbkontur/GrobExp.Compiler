using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;

namespace GrobExp.Compiler
{
    public static class LambdaExpressionCreator
    {
        public static LambdaExpression Create(Expression body, Type returnType, ParameterExpression[] parameters, string name = null, bool tailCall = false)
        {
            if (parameters.Any(parameter => parameter.IsByRef)) // TODO handle this
                return Expression.Lambda(body, name, tailCall, parameters);
            var parameterz = new ParameterExpression[parameters.Length];
            Array.Copy(parameters, parameterz, parameters.Length);
            return GetLambdaFactory(returnType, parameters)(body, name, tailCall, new ReadOnlyCollection<ParameterExpression>(parameterz));
        }

        public static LambdaExpression Create(Expression body, params ParameterExpression[] parameters)
        {
            return Create(body, body.Type, parameters);
        }

        public static Expression<TDelegate> Create<TDelegate>(Expression body, ParameterExpression[] parameters, string name = null, bool tailCall = false)
        {
            if (parameters.Any(parameter => parameter.IsByRef)) // TODO handle this
                return Expression.Lambda<TDelegate>(body, name, tailCall, parameters);
            var parameterz = new ParameterExpression[parameters.Length];
            Array.Copy(parameters, parameterz, parameters.Length);
            return (Expression<TDelegate>)GetLambdaFactory(typeof(TDelegate))(body, name, tailCall, new ReadOnlyCollection<ParameterExpression>(parameterz));
        }

        private static LambdaCreateDelegate GetLambdaFactory(Type returnType, ParameterExpression[] parameters)
        {
            return GetLambdaFactory(Extensions.GetDelegateType(parameters.Select(parameter => parameter.Type).ToArray(), returnType));
        }

        private static LambdaCreateDelegate GetLambdaFactory(Type delegateType)
        {
            var factory = (LambdaCreateDelegate)factories[delegateType];
            if(factory == null)
            {
                lock(factoriesLock)
                {
                    factory = (LambdaCreateDelegate)factories[delegateType];
                    if(factory == null)
                    {
                        factory = BuildLambdaFactory(delegateType);
                        factories[delegateType] = factory;
                    }
                }
            }
            return factory;
        }

        private static LambdaCreateDelegate BuildLambdaFactory(Type delegateType)
        {
            var resultType = typeof(Expression<>).MakeGenericType(delegateType);
            var parameterTypes = new[] {typeof(Expression), typeof(string), typeof(bool), typeof(ReadOnlyCollection<ParameterExpression>)};
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(LambdaExpression), parameterTypes, typeof(LambdaExpressionCreator), true);
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0);
                il.Ldarg(1);
                il.Ldarg(2);
                il.Ldarg(3);
                il.Newobj(resultType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single());
                il.Ret();
            }
            return (LambdaCreateDelegate)method.CreateDelegate(typeof(LambdaCreateDelegate));
        }

        private delegate LambdaExpression LambdaCreateDelegate(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters);

        private static readonly Hashtable factories = new Hashtable();
        private static readonly object factoriesLock = new object();
    }
}