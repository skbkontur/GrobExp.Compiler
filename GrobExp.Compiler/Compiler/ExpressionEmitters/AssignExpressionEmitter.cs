using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class AssignExpressionEmitter : ExpressionEmitter<BinaryExpression>
    {
        protected override bool EmitInternal(BinaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var il = context.Il;
            var left = node.Left;
            var right = node.Right;
            bool result = false;
            Type assigneeType;
            AssigneeKind assigneeKind;
            bool checkNullReferences = context.Options.HasFlag(CompilerOptions.CheckNullReferences);
            extend |= context.Options.HasFlag(CompilerOptions.ExtendOnAssign);

            GroboIL.Label assigneeIsNullLabel = null;
            bool assigneeIsNullLabelUsed = false;
            switch(left.NodeType)
            {
            case ExpressionType.Parameter:
                assigneeType = null;
                assigneeKind = AssigneeKind.Parameter;
                checkNullReferences = false;
                break;
            case ExpressionType.MemberAccess:
                var memberExpression = (MemberExpression)left;
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
                var indexExpression = (IndexExpression)left;
                if(indexExpression.Object == null)
                    throw new InvalidOperationException("Indexing of null object is invalid");
                if((indexExpression.Object.Type.IsArray && indexExpression.Object.Type.GetArrayRank() == 1) || indexExpression.Object.Type.IsList())
                {
                    if(node.NodeType != ExpressionType.Assign && context.CanReturn)
                    {
                        result |= ArrayIndexExpressionEmitter.Emit(indexExpression.Object, indexExpression.Arguments.Single(), context, returnDefaultValueLabel, ResultType.ByRefAll, extend, out assigneeType);
                        checkNullReferences = false;
                    }
                    else
                    {
                        assigneeIsNullLabel = context.CanReturn ? il.DefineLabel("assigneeIsNull") : null;
                        assigneeIsNullLabelUsed = ArrayIndexExpressionEmitter.Emit(indexExpression.Object, indexExpression.Arguments.Single(), context, assigneeIsNullLabel, ResultType.ByRefAll, extend, out assigneeType);
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
                throw new InvalidOperationException("Unable to assign to an expression of type '" + left.NodeType + "'");
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

            if(node.NodeType == ExpressionType.Assign)
            {
                if(!checkNullReferences)
                {
                    if(whatReturn == ResultType.Void)
                        EmitAssign(assigneeKind, left, context, null, right);
                    else
                    {
                        if(assigneeKind == AssigneeKind.Parameter)
                        {
                            EmitAssign(assigneeKind, left, context, null, right);
                            EmitAccess(assigneeKind, left, context);
                        }
                        else
                        {
                            context.EmitLoadArguments(right);
                            using(var value = context.DeclareLocal(right.Type))
                            {
                                il.Stloc(value);
                                EmitAssign(assigneeKind, left, context, null, value);
                                il.Ldloc(value);
                            }
                        }
                    }
                }
                else
                {
                    if(whatReturn == ResultType.Void)
                    {
                        il.Dup();
                        var skipAssigneeLabel = il.DefineLabel("skipAssignee");
                        il.Brfalse(skipAssigneeLabel);
                        EmitAssign(assigneeKind, left, context, null, right);
                        var returnLabel = il.DefineLabel("return");
                        il.Br(returnLabel);
                        context.MarkLabelAndSurroundWithSP(skipAssigneeLabel);
                        il.Pop();
                        context.MarkLabelAndSurroundWithSP(returnLabel);
                    }
                    else
                    {
                        // load value
                        var rightIsNullLabel = context.CanReturn ? il.DefineLabel("rightIsNull") : null;
                        Type valueType;
                        bool labelUsed = ExpressionEmittersCollection.Emit(right, context, rightIsNullLabel, out valueType); // stack: [address, value]
                        if(right.Type == typeof(bool) && valueType == typeof(bool?))
                            context.ConvertFromNullableBoolToBool();
                        if(labelUsed && context.CanReturn)
                            context.EmitReturnDefaultValue(right.Type, rightIsNullLabel, il.DefineLabel("rightIsNotNull"));
                        using(var value = context.DeclareLocal(right.Type))
                        {
                            il.Stloc(value);
                            il.Dup();
                            var skipAssigneeLabel = il.DefineLabel("skipAssignee");
                            il.Brfalse(skipAssigneeLabel);
                            EmitAssign(assigneeKind, left, context, null, value);
                            var returnValueLabel = il.DefineLabel("returnValue");
                            il.Br(returnValueLabel);
                            context.MarkLabelAndSurroundWithSP(skipAssigneeLabel);
                            il.Pop();
                            context.MarkLabelAndSurroundWithSP(returnValueLabel);
                            il.Ldloc(value);
                        }
                    }
                }
            }
            else
            {
                if(checkNullReferences)
                {
                    il.Dup();
                    il.Brfalse(returnDefaultValueLabel);
                    result = true;
                }
                if(assigneeType != null)
                    il.Dup();
                object[] arguments = EmitAccess(assigneeKind, left, context);
                context.EmitLoadArguments(right);
                context.EmitArithmeticOperation(GetOp(node.NodeType), left.Type, left.Type, right.Type, node.Method);
                if(whatReturn == ResultType.Void)
                    EmitAssign(assigneeKind, left, context, arguments);
                else
                {
                    if(assigneeKind == AssigneeKind.Parameter)
                    {
                        EmitAssign(assigneeKind, left, context, arguments);
                        EmitAccess(assigneeKind, left, context);
                    }
                    else
                    {
                        using(var temp = context.DeclareLocal(left.Type))
                        {
                            il.Stloc(temp);
                            EmitAssign(assigneeKind, left, context, arguments, temp);
                            il.Ldloc(temp);
                        }
                    }
                }
            }
            resultType = whatReturn == ResultType.Void ? typeof(void) : left.Type;
            if(assigneeIsNullLabelUsed)
                context.EmitReturnDefaultValue(resultType, assigneeIsNullLabel, il.DefineLabel("assigneeIsNotNull"));
            return result;
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
                    context.Il.Call(getter,indexExpression.Object.Type);
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

        private static void EmitAssign(AssigneeKind assigneeKind, Expression node, EmittingContext context, object[] arguments, Expression value)
        {
            var il = context.Il;
            switch(assigneeKind)
            {
            case AssigneeKind.Parameter:
                context.EmitLoadArguments(value);
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
                context.EmitLoadArguments(value);
                il.Stind(node.Type);
                break;
            case AssigneeKind.InstanceField:
            case AssigneeKind.StaticField:
                context.EmitLoadArguments(value);
                il.Stfld((FieldInfo)((MemberExpression)node).Member);
                break;
            case AssigneeKind.InstanceProperty:
            case AssigneeKind.StaticProperty:
                context.EmitLoadArguments(value);
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
                    context.EmitLoadArguments(value);
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
                    context.EmitLoadArguments(value);
                    MethodInfo setMethod = arrayType.GetMethod("Set");
                    if(setMethod == null)
                        throw new MissingMethodException(arrayType.ToString(), "Set");
                    context.Il.Call(setMethod, arrayType);
                }
                break;
            }
        }

        private static ExpressionType GetOp(ExpressionType nodeType)
        {
            switch(nodeType)
            {
            case ExpressionType.AddAssign:
                return ExpressionType.Add;
            case ExpressionType.AddAssignChecked:
                return ExpressionType.AddChecked;
            case ExpressionType.SubtractAssign:
                return ExpressionType.Subtract;
            case ExpressionType.SubtractAssignChecked:
                return ExpressionType.SubtractChecked;
            case ExpressionType.MultiplyAssign:
                return ExpressionType.Multiply;
            case ExpressionType.MultiplyAssignChecked:
                return ExpressionType.MultiplyChecked;
            case ExpressionType.DivideAssign:
                return ExpressionType.Divide;
            case ExpressionType.ModuloAssign:
                return ExpressionType.Modulo;
            case ExpressionType.PowerAssign:
                return ExpressionType.Power;
            case ExpressionType.AndAssign:
                return ExpressionType.And;
            case ExpressionType.OrAssign:
                return ExpressionType.Or;
            case ExpressionType.ExclusiveOrAssign:
                return ExpressionType.ExclusiveOr;
            case ExpressionType.LeftShiftAssign:
                return ExpressionType.LeftShift;
            case ExpressionType.RightShiftAssign:
                return ExpressionType.RightShift;
            default:
                throw new NotSupportedException("Unable to extract operation type from node type '" + nodeType + "'");
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