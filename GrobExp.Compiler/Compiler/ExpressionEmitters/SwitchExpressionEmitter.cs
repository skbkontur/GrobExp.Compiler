using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class SwitchExpressionEmitter : ExpressionEmitter<SwitchExpression>
    {
        protected override bool EmitInternal(SwitchExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            GroboIL il = context.Il;
            var defaultLabel = il.DefineLabel("default");
            var caseLabels = new GroboIL.Label[node.Cases.Count];
            GroboIL.Label switchValueIsNullLabel = null;
            for(int index = 0; index < node.Cases.Count; index++)
                caseLabels[index] = il.DefineLabel("case#" + index);
            context.EmitLoadArguments(node.SwitchValue);
            using(var switchValue = context.DeclareLocal(node.SwitchValue.Type))
            {
                il.Stloc(switchValue);
                Tuple<FieldInfo, FieldInfo, int> switchCase;
                if(context.Switches.TryGetValue(node, out switchCase))
                {
                    // use simplified hashtable to locate the proper case
                    var labels = new List<GroboIL.Label>();
                    for (int index = 0; index < node.Cases.Count; index++)
                    {
                        foreach (var testValue in node.Cases[index].TestValues)
                        {
                            if (((ConstantExpression)testValue).Value != null)
                                labels.Add(caseLabels[index]);
                            else
                                switchValueIsNullLabel = caseLabels[index];
                        }
                    }
                    if (switchValueIsNullLabel != null)
                    {
                        if (!node.SwitchValue.Type.IsNullable())
                            il.Ldloc(switchValue);
                        else
                        {
                            il.Ldloca(switchValue);
                            context.EmitHasValueAccess(node.SwitchValue.Type);
                        }
                        il.Brfalse(switchValueIsNullLabel);
                    }
                    EmittingContext.LocalHolder pureSwitchValue = switchValue;
                    if (node.SwitchValue.Type.IsNullable())
                    {
                        pureSwitchValue = context.DeclareLocal(node.SwitchValue.Type.GetGenericArguments()[0]);
                        il.Ldloca(switchValue);
                        context.EmitValueAccess(node.SwitchValue.Type);
                        il.Stloc(pureSwitchValue);
                    }
                    Type constantsType;
                    ExpressionEmittersCollection.Emit(context.ConstantsParameter, context, out constantsType);
                    il.Ldfld(switchCase.Item1);
                    var type = node.SwitchValue.Type.IsNullable() ? node.SwitchValue.Type.GetGenericArguments()[0] : node.SwitchValue.Type;
                    var typeCode = Type.GetTypeCode(type);
                    switch(typeCode)
                    {
                    case TypeCode.Byte:
                    case TypeCode.Char:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        il.Ldloc(pureSwitchValue);
                        break;
                    default:
                        if(type.IsValueType)
                            il.Ldloca(pureSwitchValue);
                        else
                            il.Ldloc(pureSwitchValue);
                        il.Call(typeof(object).GetMethod("GetHashCode"), type);
                        break;
                    }
                    using(var index = context.DeclareLocal(typeof(int)))
                    {
                        if(typeCode == TypeCode.Int64 || typeCode == TypeCode.UInt64)
                            il.Ldc_I8(switchCase.Item3);
                        else
                            il.Ldc_I4(switchCase.Item3);
                        il.Rem(true);
                        if (typeCode == TypeCode.Int64 || typeCode == TypeCode.UInt64)
                            il.Conv<int>();
                        il.Stloc(index);
                        il.Ldloc(index);
                        il.Ldelem(type);
                        il.Ldloc(pureSwitchValue);
                        if(node.Comparison != null)
                            il.Call(node.Comparison);
                        else
                            il.Ceq();
                        il.Brfalse(defaultLabel);
                        ExpressionEmittersCollection.Emit(context.ConstantsParameter, context, out constantsType);
                        il.Ldfld(switchCase.Item2);
                        il.Ldloc(index);
                        il.Ldelem(typeof(int));
                        il.Switch(labels.ToArray());
                    }
                    if (pureSwitchValue != switchValue)
                        pureSwitchValue.Dispose();
                }
                else
                {
                    // use a number of if/else branches to locate the proper case
                    EmittingContext.LocalHolder pureSwitchValue = switchValue;
                    EmittingContext.LocalHolder switchValueIsNull = null;
                    if (node.SwitchValue.Type.IsNullable())
                    {
                        pureSwitchValue = context.DeclareLocal(node.SwitchValue.Type.GetGenericArguments()[0]);
                        switchValueIsNull = context.DeclareLocal(typeof(bool));
                        il.Ldloca(switchValue);
                        il.Dup();
                        context.EmitValueAccess(node.SwitchValue.Type);
                        il.Stloc(pureSwitchValue);
                        context.EmitHasValueAccess(node.SwitchValue.Type);
                        il.Stloc(switchValueIsNull);
                    }
                    for(int index = 0; index < node.Cases.Count; index++)
                    {
                        var caSe = node.Cases[index];
                        var label = caseLabels[index];
                        foreach(var testValue in caSe.TestValues)
                        {
                            context.EmitLoadArguments(testValue);
                            GroboIL.Label elseLabel = null;
                            if(testValue.Type.IsNullable())
                            {
                                elseLabel = il.DefineLabel("else");
                                using(var temp = context.DeclareLocal(testValue.Type))
                                {
                                    il.Stloc(temp);
                                    il.Ldloca(temp);
                                    context.EmitHasValueAccess(testValue.Type);
                                    if(switchValueIsNull != null)
                                    {
                                        il.Ldloc(switchValueIsNull);
                                        il.Or();
                                        il.Brfalse(label);
                                        il.Ldloca(temp);
                                        context.EmitHasValueAccess(testValue.Type);
                                        il.Ldloc(switchValueIsNull);
                                        il.And();
                                    }
                                    il.Brfalse(elseLabel);
                                    il.Ldloca(temp);
                                    context.EmitValueAccess(testValue.Type);
                                }
                            }
                            il.Ldloc(pureSwitchValue);
                            if(node.Comparison != null)
                                il.Call(node.Comparison);
                            else
                                il.Ceq();
                            il.Brtrue(label);
                            if(elseLabel != null)
                                context.MarkLabelAndSurroundWithSP(elseLabel);
                        }
                    }
                }
            }
            context.MarkLabelAndSurroundWithSP(defaultLabel);
            var doneLabel = il.DefineLabel("done");
            context.EmitLoadArguments(node.DefaultBody);
            il.Br(doneLabel);
            for(int index = 0; index < node.Cases.Count; ++index)
            {
                context.MarkLabelAndSurroundWithSP(caseLabels[index]);
                context.EmitLoadArguments(node.Cases[index].Body);
                if(index < node.Cases.Count - 1)
                    il.Br(doneLabel);
            }
            context.MarkLabelAndSurroundWithSP(doneLabel);
            resultType = node.Type;
            return false;
        }
    }
}