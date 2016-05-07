using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class UnaryAssignExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        protected override bool EmitInternal(UnaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            GroboIL il = context.Il;
            var result = false;
            var operand = node.Operand;
            Type assigneeType;
            AssigneeKind assigneeKind;
            bool checkNullReferences = context.Options.HasFlag(CompilerOptions.CheckNullReferences);
            extend |= context.Options.HasFlag(CompilerOptions.ExtendOnAssign);

            GroboIL.Label assigneeIsNullLabel = null;
            bool assigneeIsNullLabelUsed = false;
            switch(operand.NodeType)
            {
            case ExpressionType.Parameter:
                assigneeType = null;
                assigneeKind = AssigneeKind.Parameter;
                checkNullReferences = false;
                break;
            case ExpressionType.MemberAccess:
                var memberExpression = (MemberExpression)operand;
                if(memberExpression.Expression == null)
                {
                    assigneeType = null;
                    assigneeKind = memberExpression.Member is FieldInfo ? AssigneeKind.StaticField : AssigneeKind.StaticProperty;
                    checkNullReferences = false;
                }
                else
                {
                    bool closureAssign = memberExpression.Expression == context.ClosureParameter;
                    checkNullReferences &= !closureAssign;
                    if(node.NodeType != ExpressionType.Assign && context.CanReturn)
                        result |= ExpressionEmittersCollection.Emit(memberExpression.Expression, context, returnDefaultValueLabel, ResultType.ByRefValueTypesOnly, extend, out assigneeType);
                    else
                    {
                        assigneeIsNullLabel = !closureAssign && context.CanReturn ? il.DefineLabel("assigneeIsNull") : null;
                        assigneeIsNullLabelUsed = ExpressionEmittersCollection.Emit(memberExpression.Expression, context, assigneeIsNullLabel, ResultType.ByRefValueTypesOnly, extend, out assigneeType);
                    }
                    assigneeKind = memberExpression.Member is FieldInfo ? AssigneeKind.InstanceField : AssigneeKind.InstanceProperty;
                }
                break;
            case ExpressionType.Index:
                var indexExpression = (IndexExpression)operand;
                if(indexExpression.Object == null)
                    throw new InvalidOperationException("Indexing of null object is invalid");
                if(indexExpression.Object.Type.IsArray && indexExpression.Object.Type.GetArrayRank() == 1)
                {
                    if(node.NodeType != ExpressionType.Assign && context.CanReturn)
                    {
                        result |= ExpressionEmittersCollection.Emit(Expression.ArrayIndex(indexExpression.Object, indexExpression.Arguments.Single()), context, returnDefaultValueLabel, ResultType.ByRefAll, extend, out assigneeType);
                        checkNullReferences = false;
                    }
                    else
                    {
                        assigneeIsNullLabel = context.CanReturn ? il.DefineLabel("assigneeIsNull") : null;
                        assigneeIsNullLabelUsed = ExpressionEmittersCollection.Emit(Expression.ArrayIndex(indexExpression.Object, indexExpression.Arguments.Single()), context, assigneeIsNullLabel, ResultType.ByRefAll, extend, out assigneeType);
                    }
                    assigneeKind = AssigneeKind.SimpleArray;
                }
                else
                {
                    if(node.NodeType != ExpressionType.Assign && context.CanReturn)
                        result |= ExpressionEmittersCollection.Emit(indexExpression.Object, context, returnDefaultValueLabel, ResultType.ByRefValueTypesOnly, extend, out assigneeType);
                    else
                    {
                        assigneeIsNullLabel = context.CanReturn ? il.DefineLabel("assigneeIsNull") : null;
                        assigneeIsNullLabelUsed = ExpressionEmittersCollection.Emit(indexExpression.Object, context, assigneeIsNullLabel, ResultType.ByRefValueTypesOnly, extend, out assigneeType);
                    }
                    assigneeKind = indexExpression.Indexer != null ? AssigneeKind.IndexedProperty : AssigneeKind.MultiDimensionalArray;
                }
                break;
            default:
                throw new InvalidOperationException("Unable to assign to an expression of type '" + operand.NodeType + "'");
            }
            if(assigneeType != null && assigneeType.IsValueType)
            {
                using(var temp = context.DeclareLocal(assigneeType))
                {
                    il.Stloc(temp);
                    il.Ldloca(temp);
                }
                assigneeType = assigneeType.MakeByRefType();
            }
            if(assigneeIsNullLabelUsed)
                context.EmitReturnDefaultValue(assigneeType, assigneeIsNullLabel, il.DefineLabel("assigneeIsNotNull"));

            if(checkNullReferences)
            {
                il.Dup();
                il.Brfalse(returnDefaultValueLabel);
                result = true;
            }

            if(assigneeType != null)
                il.Dup();
            object[] arguments = EmitAccess(assigneeKind, operand, context);
            if(!operand.Type.IsNullable())
            {
                if(whatReturn == ResultType.Void)
                {
                    EmitOp(node.NodeType, node.Method, node.Type, context);
                    EmitAssign(assigneeKind, operand, context, arguments);
                }
                else
                {
                    if(node.NodeType == ExpressionType.PostDecrementAssign || node.NodeType == ExpressionType.PostIncrementAssign)
                    {
                        using(var assignmentResult = context.DeclareLocal(operand.Type))
                        {
                            il.Stloc(assignmentResult);
                            il.Ldloc(assignmentResult);
                            EmitOp(node.NodeType, node.Method, node.Type, context);
                            EmitAssign(assigneeKind, operand, context, arguments);
                            il.Ldloc(assignmentResult);
                        }
                    }
                    else
                    {
                        EmitOp(node.NodeType, node.Method, node.Type, context);
                        using(var assignmentResult = context.DeclareLocal(operand.Type))
                        {
                            il.Stloc(assignmentResult);
                            EmitAssign(assigneeKind, operand, context, arguments, assignmentResult);
                            il.Ldloc(assignmentResult);
                        }
                    }
                }
            }
            else
            {
                using(var value = context.DeclareLocal(operand.Type))
                {
                    il.Stloc(value);
                    il.Ldloca(value);
                    context.EmitHasValueAccess(operand.Type);
                    var returnNullLabel = il.DefineLabel("returnNull");
                    il.Brfalse(returnNullLabel);
                    il.Ldloca(value);
                    context.EmitValueAccess(operand.Type);
                    Type argumentType = operand.Type.GetGenericArguments()[0];
                    ConstructorInfo constructor = operand.Type.GetConstructor(new[] {argumentType});
                    if(whatReturn == ResultType.Void)
                    {
                        EmitOp(node.NodeType, node.Method, argumentType, context);
                        il.Newobj(constructor);
                        EmitAssign(assigneeKind, operand, context, arguments);
                    }
                    else
                    {
                        if(node.NodeType == ExpressionType.PostDecrementAssign || node.NodeType == ExpressionType.PostIncrementAssign)
                        {
                            EmitOp(node.NodeType, node.Method, argumentType, context);
                            il.Newobj(constructor);
                            EmitAssign(assigneeKind, operand, context, arguments);
                            il.Ldloc(value);
                        }
                        else
                        {
                            EmitOp(node.NodeType, node.Method, argumentType, context);
                            il.Newobj(constructor);
                            using(var assignmentResult = context.DeclareLocal(operand.Type))
                            {
                                il.Stloc(assignmentResult);
                                EmitAssign(assigneeKind, operand, context, arguments, assignmentResult);
                                il.Ldloc(assignmentResult);
                            }
                        }
                    }
                    var doneLabel = il.DefineLabel("done");
                    il.Br(doneLabel);
                    context.MarkLabelAndSurroundWithSP(returnNullLabel);
                    if(assigneeType != null)
                        il.Pop();
                    if(whatReturn != ResultType.Void)
                        il.Ldloc(value);
                    context.MarkLabelAndSurroundWithSP(doneLabel);
                }
            }
            resultType = whatReturn == ResultType.Void ? typeof(void) : operand.Type;
            return result;
        }

        private static void EmitOp(ExpressionType nodeType, MethodInfo method, Type type, EmittingContext context)
        {
            var il = context.Il;
            if(method != null)
                il.Call(method);
            else
            {
                switch(nodeType)
                {
                case ExpressionType.PostIncrementAssign:
                    il.Ldc_I4(1);
                    context.EmitConvert(typeof(int), type);
                    il.Add();
                    break;
                case ExpressionType.PostDecrementAssign:
                    il.Ldc_I4(1);
                    context.EmitConvert(typeof(int), type);
                    il.Sub();
                    break;
                case ExpressionType.PreIncrementAssign:
                    il.Ldc_I4(1);
                    context.EmitConvert(typeof(int), type);
                    il.Add();
                    break;
                case ExpressionType.PreDecrementAssign:
                    il.Ldc_I4(1);
                    context.EmitConvert(typeof(int), type);
                    il.Sub();
                    break;
                }
            }
        }

        private static object[] EmitAccess(AssigneeKind assigneeKind, Expression node, EmittingContext context)
        {
            object[] arguments = null;
            var il = context.Il;
            switch(assigneeKind)
            {
            case AssigneeKind.Parameter:
                var index = Array.IndexOf(context.Parameters, node);
                if(index >= 0)
                    il.Ldarg(index);
                else
                {
                    GroboIL.Local variable;
                    if(context.VariablesToLocals.TryGetValue((ParameterExpression)node, out variable))
                        il.Ldloc(variable);
                    else
                        throw new InvalidOperationException("Unknown parameter " + node);
                }
                break;
            case AssigneeKind.InstanceField:
            case AssigneeKind.StaticField:
                il.Ldfld((FieldInfo)((MemberExpression)node).Member);
                break;
            case AssigneeKind.InstanceProperty:
            case AssigneeKind.StaticProperty:
                var memberExpression = (MemberExpression)node;
                il.Call(((PropertyInfo)memberExpression.Member).GetGetMethod(context.SkipVisibility), memberExpression.Expression == null ? null : memberExpression.Expression.Type);
                break;
            case AssigneeKind.SimpleArray:
                il.Ldind(node.Type);
                break;
            case AssigneeKind.IndexedProperty:
                {
                    var indexExpression = (IndexExpression)node;
                    var args = new List<object>();
                    foreach(var argument in indexExpression.Arguments)
                    {
                        context.EmitLoadArguments(argument);
                        if(argument.NodeType == ExpressionType.Constant || (argument.NodeType == ExpressionType.MemberAccess && ((MemberExpression)argument).Member.MemberType == MemberTypes.Field && ((FieldInfo)((MemberExpression)argument).Member).IsStatic))
                            args.Add(argument);
                        else
                        {
                            var local = context.DeclareLocal(argument.Type);
                            args.Add(local);
                            il.Stloc(local);
                            il.Ldloc(local);
                        }
                    }
                    arguments = args.ToArray();
                    MethodInfo getter = indexExpression.Indexer.GetGetMethod(context.SkipVisibility);
                    if(getter == null)
                        throw new MissingMethodException(indexExpression.Indexer.ReflectedType.ToString(), "get_" + indexExpression.Indexer.Name);
                    context.Il.Call(getter, indexExpression.Object.Type);
                }
                break;
            case AssigneeKind.MultiDimensionalArray:
                {
                    var indexExpression = (IndexExpression)node;
                    Type arrayType = indexExpression.Object.Type;
                    if(!arrayType.IsArray)
                        throw new InvalidOperationException("An array expected");
                    int rank = arrayType.GetArrayRank();
                    if(rank != indexExpression.Arguments.Count)
                        throw new InvalidOperationException("Incorrect number of indeces '" + indexExpression.Arguments.Count + "' provided to access an array with rank '" + rank + "'");
                    Type indexType = indexExpression.Arguments.First().Type;
                    if(indexType != typeof(int))
                        throw new InvalidOperationException("Indexing array with an index of type '" + indexType + "' is not allowed");
                    var args = new List<object>();
                    foreach(var argument in indexExpression.Arguments)
                    {
                        context.EmitLoadArguments(argument);
                        if(argument.NodeType == ExpressionType.Constant || (argument.NodeType == ExpressionType.MemberAccess && ((MemberExpression)argument).Member.MemberType == MemberTypes.Field && ((FieldInfo)((MemberExpression)argument).Member).IsStatic))
                            args.Add(argument);
                        else
                        {
                            var local = context.DeclareLocal(argument.Type);
                            args.Add(local);
                            il.Stloc(local);
                            il.Ldloc(local);
                        }
                    }
                    arguments = args.ToArray();
                    MethodInfo getMethod = arrayType.GetMethod("Get");
                    if(getMethod == null)
                        throw new MissingMethodException(arrayType.ToString(), "Get");
                    context.Il.Call(getMethod, arrayType);
                }
                break;
            }
            return arguments;
        }

        private static void EmitAssign(AssigneeKind assigneeKind, Expression node, EmittingContext context, object[] arguments)
        {
            var il = context.Il;
            switch(assigneeKind)
            {
            case AssigneeKind.Parameter:
                var index = Array.IndexOf(context.Parameters, node);
                if(index >= 0)
                    il.Starg(index);
                else
                {
                    GroboIL.Local variable;
                    if(context.VariablesToLocals.TryGetValue((ParameterExpression)node, out variable))
                        il.Stloc(variable);
                    else
                        throw new InvalidOperationException("Unknown parameter " + node);
                }
                break;
            case AssigneeKind.SimpleArray:
                il.Stind(node.Type);
                break;
            case AssigneeKind.InstanceField:
            case AssigneeKind.StaticField:
                il.Stfld((FieldInfo)((MemberExpression)node).Member);
                break;
            case AssigneeKind.InstanceProperty:
            case AssigneeKind.StaticProperty:
                var memberExpression = (MemberExpression)node;
                il.Call(((PropertyInfo)memberExpression.Member).GetSetMethod(context.SkipVisibility), memberExpression.Expression == null ? null : memberExpression.Expression.Type);
                break;
            case AssigneeKind.IndexedProperty:
                {
                    using(var temp = context.DeclareLocal(node.Type))
                    {
                        il.Stloc(temp);
                        var indexExpression = (IndexExpression)node;
                        if(arguments == null)
                            context.EmitLoadArguments(indexExpression.Arguments.ToArray());
                        else
                        {
                            foreach(var argument in arguments)
                            {
                                if(argument is Expression)
                                    context.EmitLoadArguments((Expression)argument);
                                else
                                {
                                    var local = (EmittingContext.LocalHolder)argument;
                                    il.Ldloc(local);
                                    local.Dispose();
                                }
                            }
                        }
                        il.Ldloc(temp);
                        MethodInfo setter = indexExpression.Indexer.GetSetMethod(context.SkipVisibility);
                        if(setter == null)
                            throw new MissingMethodException(indexExpression.Indexer.ReflectedType.ToString(), "set_" + indexExpression.Indexer.Name);
                        context.Il.Call(setter, indexExpression.Object.Type);
                    }
                }
                break;
            case AssigneeKind.MultiDimensionalArray:
                {
                    using(var temp = context.DeclareLocal(node.Type))
                    {
                        il.Stloc(temp);
                        var indexExpression = (IndexExpression)node;
                        Type arrayType = indexExpression.Object.Type;
                        if(!arrayType.IsArray)
                            throw new InvalidOperationException("An array expected");
                        int rank = arrayType.GetArrayRank();
                        if(rank != indexExpression.Arguments.Count)
                            throw new InvalidOperationException("Incorrect number of indeces '" + indexExpression.Arguments.Count + "' provided to access an array with rank '" + rank + "'");
                        Type indexType = indexExpression.Arguments.First().Type;
                        if(indexType != typeof(int))
                            throw new InvalidOperationException("Indexing array with an index of type '" + indexType + "' is not allowed");
                        if(arguments == null)
                            context.EmitLoadArguments(indexExpression.Arguments.ToArray());
                        else
                        {
                            foreach(var argument in arguments)
                            {
                                if(argument is Expression)
                                    context.EmitLoadArguments((Expression)argument);
                                else
                                {
                                    var local = (EmittingContext.LocalHolder)argument;
                                    il.Ldloc(local);
                                    local.Dispose();
                                }
                            }
                        }
                        il.Ldloc(temp);
                        MethodInfo setMethod = arrayType.GetMethod("Set");
                        if(setMethod == null)
                            throw new MissingMethodException(arrayType.ToString(), "Set");
                        context.Il.Call(setMethod, arrayType);
                    }
                }
                break;
            }
        }

        private static void EmitAssign(AssigneeKind assigneeKind, Expression node, EmittingContext context, object[] arguments, EmittingContext.LocalHolder value)
        {
            var il = context.Il;
            switch(assigneeKind)
            {
            case AssigneeKind.Parameter:
                il.Ldloc(value);
                var index = Array.IndexOf(context.Parameters, node);
                if(index >= 0)
                    il.Starg(index);
                else
                {
                    GroboIL.Local variable;
                    if(context.VariablesToLocals.TryGetValue((ParameterExpression)node, out variable))
                        il.Stloc(variable);
                    else
                        throw new InvalidOperationException("Unknown parameter " + node);
                }
                break;
            case AssigneeKind.SimpleArray:
                il.Ldloc(value);
                il.Stind(node.Type);
                break;
            case AssigneeKind.InstanceField:
            case AssigneeKind.StaticField:
                il.Ldloc(value);
                il.Stfld((FieldInfo)((MemberExpression)node).Member);
                break;
            case AssigneeKind.InstanceProperty:
            case AssigneeKind.StaticProperty:
                il.Ldloc(value);
                var memberExpression = (MemberExpression)node;
                il.Call(((PropertyInfo)memberExpression.Member).GetSetMethod(context.SkipVisibility), memberExpression.Expression == null ? null : memberExpression.Expression.Type);
                break;
            case AssigneeKind.IndexedProperty:
                {
                    var indexExpression = (IndexExpression)node;
                    if(arguments == null)
                        context.EmitLoadArguments(indexExpression.Arguments.ToArray());
                    else
                    {
                        foreach(var argument in arguments)
                        {
                            if(argument is Expression)
                                context.EmitLoadArguments((Expression)argument);
                            else
                            {
                                var local = (EmittingContext.LocalHolder)argument;
                                il.Ldloc(local);
                                local.Dispose();
                            }
                        }
                    }
                    il.Ldloc(value);
                    MethodInfo setter = indexExpression.Indexer.GetSetMethod(context.SkipVisibility);
                    if(setter == null)
                        throw new MissingMethodException(indexExpression.Indexer.ReflectedType.ToString(), "set_" + indexExpression.Indexer.Name);
                    context.Il.Call(setter, indexExpression.Object.Type);
                }
                break;
            case AssigneeKind.MultiDimensionalArray:
                {
                    var indexExpression = (IndexExpression)node;
                    Type arrayType = indexExpression.Object.Type;
                    if(!arrayType.IsArray)
                        throw new InvalidOperationException("An array expected");
                    int rank = arrayType.GetArrayRank();
                    if(rank != indexExpression.Arguments.Count)
                        throw new InvalidOperationException("Incorrect number of indeces '" + indexExpression.Arguments.Count + "' provided to access an array with rank '" + rank + "'");
                    Type indexType = indexExpression.Arguments.First().Type;
                    if(indexType != typeof(int))
                        throw new InvalidOperationException("Indexing array with an index of type '" + indexType + "' is not allowed");
                    if(arguments == null)
                        context.EmitLoadArguments(indexExpression.Arguments.ToArray());
                    else
                    {
                        foreach(var argument in arguments)
                        {
                            if(argument is Expression)
                                context.EmitLoadArguments((Expression)argument);
                            else
                            {
                                var local = (EmittingContext.LocalHolder)argument;
                                il.Ldloc(local);
                                local.Dispose();
                            }
                        }
                    }
                    il.Ldloc(value);
                    MethodInfo setMethod = arrayType.GetMethod("Set");
                    if(setMethod == null)
                        throw new MissingMethodException(arrayType.ToString(), "Set");
                    context.Il.Call(setMethod, arrayType);
                }
                break;
            }
        }

        private enum AssigneeKind
        {
            Parameter,
            InstanceField,
            InstanceProperty,
            StaticField,
            StaticProperty,
            SimpleArray,
            MultiDimensionalArray,
            IndexedProperty
        }
    }
}