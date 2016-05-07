using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class UnaryAritmeticOperationExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        protected override bool EmitInternal(UnaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            Type operandType;
            var result = ExpressionEmittersCollection.Emit(node.Operand, context, returnDefaultValueLabel, ResultType.Value, extend, out operandType);
            GroboIL il = context.Il;
            if(node.NodeType == ExpressionType.IsTrue || node.NodeType == ExpressionType.IsFalse)
            {
                if(!operandType.IsNullable())
                {
                    if(node.Method != null)
                        il.Call(node.Method);
                    else if(operandType == typeof(bool))
                    {
                        if(node.NodeType == ExpressionType.IsFalse)
                        {
                            il.Ldc_I4(1);
                            il.Xor();
                        }
                    }
                    else throw new InvalidOperationException("Cannot perform operation '" + node.NodeType + "' to a type '" + operandType + "'");
                }
                else
                {
                    using(var temp = context.DeclareLocal(operandType))
                    {
                        il.Stloc(temp);
                        il.Ldloca(temp);
                        context.EmitHasValueAccess(operandType);
                        var returnFalseLabel = il.DefineLabel("returnFalse");
                        il.Brfalse(returnFalseLabel);
                        il.Ldloca(temp);
                        context.EmitValueAccess(operandType);
                        if(node.Method != null)
                            il.Call(node.Method);
                        else if(operandType == typeof(bool?))
                        {
                            if(node.NodeType == ExpressionType.IsFalse)
                            {
                                il.Ldc_I4(1);
                                il.Xor();
                            }
                        }
                        else throw new InvalidOperationException("Cannot perform operation '" + node.NodeType + "' to a type '" + operandType + "'");
                        var doneLabel = il.DefineLabel("done");
                        il.Br(doneLabel);
                        context.MarkLabelAndSurroundWithSP(returnFalseLabel);
                        il.Ldc_I4(0);
                        context.MarkLabelAndSurroundWithSP(doneLabel);
                    }
                }
            }
            else
            {
                if(!operandType.IsNullable())
                {
                    if(node.Method != null)
                        il.Call(node.Method);
                    else
                    {
                        if(operandType.IsStruct())
                            throw new InvalidOperationException("Cannot perform operation '" + node.NodeType + "' to a struct '" + operandType + "'");
                        switch(node.NodeType)
                        {
                        case ExpressionType.UnaryPlus:
                            break;
                        case ExpressionType.Negate:
                            il.Neg();
                            break;
                        case ExpressionType.NegateChecked:
                            using(var temp = context.DeclareLocal(operandType))
                            {
                                il.Stloc(temp);
                                il.Ldc_I4(0);
                                context.EmitConvert(typeof(int), operandType);
                                il.Ldloc(temp);
                                il.Sub_Ovf(operandType.Unsigned());
                            }
                            break;
                        case ExpressionType.Increment:
                            il.Ldc_I4(1);
                            context.EmitConvert(typeof(int), operandType);
                            il.Add();
                            break;
                        case ExpressionType.Decrement:
                            il.Ldc_I4(1);
                            context.EmitConvert(typeof(int), operandType);
                            il.Sub();
                            break;
                        case ExpressionType.OnesComplement:
                            il.Not();
                            break;
                        default:
                            throw new InvalidOperationException("Node type '" + node.NodeType + "' invalid at this point");
                        }
                    }
                }
                else
                {
                    using(var temp = context.DeclareLocal(operandType))
                    {
                        il.Stloc(temp);
                        il.Ldloca(temp);
                        context.EmitHasValueAccess(operandType);
                        var returnNullLabel = il.DefineLabel("returnLabel");
                        il.Brfalse(returnNullLabel);
                        Type argumentType = operandType.GetGenericArguments()[0];
                        if(node.Method != null)
                        {
                            il.Ldloca(temp);
                            context.EmitValueAccess(operandType);
                            il.Call(node.Method);
                        }
                        else
                        {
                            switch(node.NodeType)
                            {
                            case ExpressionType.UnaryPlus:
                                il.Ldloca(temp);
                                context.EmitValueAccess(operandType);
                                break;
                            case ExpressionType.Negate:
                                il.Ldloca(temp);
                                context.EmitValueAccess(operandType);
                                il.Neg();
                                break;
                            case ExpressionType.NegateChecked:
                                il.Ldc_I4(0);
                                context.EmitConvert(typeof(int), argumentType);
                                il.Ldloca(temp);
                                context.EmitValueAccess(operandType);
                                il.Sub_Ovf(argumentType.Unsigned());
                                break;
                            case ExpressionType.Increment:
                                il.Ldloca(temp);
                                context.EmitValueAccess(operandType);
                                il.Ldc_I4(1);
                                context.EmitConvert(typeof(int), argumentType);
                                il.Add();
                                break;
                            case ExpressionType.Decrement:
                                il.Ldloca(temp);
                                context.EmitValueAccess(operandType);
                                il.Ldc_I4(1);
                                context.EmitConvert(typeof(int), argumentType);
                                il.Sub();
                                break;
                            case ExpressionType.OnesComplement:
                                il.Ldloca(temp);
                                context.EmitValueAccess(operandType);
                                il.Not();
                                break;
                            default:
                                throw new InvalidOperationException("Node type '" + node.NodeType + "' invalid at this point");
                            }
                        }
                        il.Newobj(operandType.GetConstructor(new[] {argumentType}));
                        var doneLabel = il.DefineLabel("done");
                        il.Br(doneLabel);
                        context.MarkLabelAndSurroundWithSP(returnNullLabel);
                        context.EmitLoadDefaultValue(operandType);
                        context.MarkLabelAndSurroundWithSP(doneLabel);
                    }
                }
            }
            resultType = node.Type;
            return result;
        }
    }
}