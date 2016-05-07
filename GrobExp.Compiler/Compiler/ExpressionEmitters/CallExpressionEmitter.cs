using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class CallExpressionEmitter : ExpressionEmitter<MethodCallExpression>
    {
        protected override bool EmitInternal(MethodCallExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var result = false;
            GroboIL il = context.Il;
            var method = node.Method;
            Expression obj;
            IEnumerable<Expression> arguments;
            bool isStatic = method.IsStatic;
            if(!isStatic)
            {
                obj = node.Object;
                arguments = node.Arguments;
            }
            else if(method.GetCustomAttributes(typeof(ExtensionAttribute), false).Any())
            {
                obj = node.Arguments[0];
                arguments = node.Arguments.Skip(1);
            }
            else
            {
                obj = null;
                arguments = node.Arguments;
            }
            Type type = obj == null ? null : obj.Type;
            if(obj != null)
            {
                Type actualType;
                result |= ExpressionEmittersCollection.Emit(obj, context, returnDefaultValueLabel, isStatic ? ResultType.Value : ResultType.ByRefValueTypesOnly, extend, out actualType); // stack: [obj]
                if(actualType == typeof(void))
                    throw new InvalidOperationException("Unable to call method on void");
                if(actualType.IsValueType && !isStatic)
                {
                    using(var temp = context.DeclareLocal(actualType))
                    {
                        il.Stloc(temp);
                        il.Ldloca(temp);
                    }
                    actualType = actualType.MakeByRefType();
                }
                if(context.Options.HasFlag(CompilerOptions.CheckNullReferences) && !actualType.IsValueType)
                {
                    if(method.DeclaringType != typeof(Enumerable))
                        result |= context.EmitNullChecking(type, returnDefaultValueLabel);
                    else
                    {
                        var arrIsNotNullLabel = il.DefineLabel("arrIsNotNull");
                        il.Dup();
                        il.Brtrue(arrIsNotNullLabel);
                        il.Pop();
                        il.Ldc_I4(0);
                        il.Newarr(GetElementType(type));
                        context.MarkLabelAndSurroundWithSP(arrIsNotNullLabel);
                    }
                }
            }
            var parameters = method.GetParameters();
            var argumentsArray = arguments.ToArray();
            for(int i = 0; i < argumentsArray.Length; i++)
            {
                var argument = argumentsArray[i];
                var parameter = parameters[i];
                if(parameter.ParameterType.IsByRef)
                {
                    Type argumentType;
                    var options = context.Options;
                    context.Options = CompilerOptions.None;
                    ExpressionEmittersCollection.Emit(argument, context, null, ResultType.ByRefAll, false, out argumentType);
                    context.Options = options;
                    if(!argumentType.IsByRef)
                        throw new InvalidOperationException("Expected type by reference");
                }
                else
                {
                    Type argumentType;
                    context.EmitLoadArgument(argument, true, out argumentType);
                }
            }
            il.Call(method, type);
            resultType = node.Type;
            return result;
        }

        private static Type GetElementType(Type type)
        {
            if(type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];
            if(type == typeof(IEnumerable))
                return typeof(object);
            var interfaces = type.GetInterfaces();
            foreach(var interfaCe in interfaces)
            {
                if(interfaCe.IsGenericType && interfaCe.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return interfaCe.GetGenericArguments()[0];
            }
            if(interfaces.Any(interfaCe => interfaCe == typeof(IEnumerable)))
                return typeof(object);
            throw new InvalidOperationException("Unable to extract element type from type '" + type + "'");
        }
    }
}