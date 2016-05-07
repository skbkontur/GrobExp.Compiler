using System;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class ConvertExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        protected override bool EmitInternal(UnaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var il = context.Il;
            GroboIL.Label operandIsNullLabel = context.CanReturn ? il.DefineLabel("operandIsNull") : null;
            var operandIsNullLabelUsed = ExpressionEmittersCollection.Emit(node.Operand, context, operandIsNullLabel, ResultType.Value, extend, out resultType); // stack: [obj]
            if(operandIsNullLabelUsed)
                context.EmitReturnDefaultValue(resultType, operandIsNullLabel, il.DefineLabel("operandIsNotNull"));
            if(resultType != node.Type && !(context.Options.HasFlag(CompilerOptions.UseTernaryLogic) && resultType == typeof(bool?) && node.Type == typeof(bool)))
            {
                if(node.Method != null)
                {
                    if(!resultType.IsNullable())
                    {
                        il.Call(node.Method);
                        if(node.Type.IsNullable())
                            il.Newobj(node.Type.GetConstructor(new[] {node.Method.ReturnType}));
                    }
                    else
                    {
                        using(var temp = context.DeclareLocal(resultType))
                        {
                            il.Stloc(temp);
                            il.Ldloca(temp);
                            il.Dup();
                            context.EmitHasValueAccess(resultType);
                            var valueIsNullLabel = operandIsNullLabelUsed ? operandIsNullLabel : il.DefineLabel("valueIsNull");
                            il.Brfalse(valueIsNullLabel);
                            context.EmitValueAccess(resultType);
                            il.Call(node.Method);
                            il.Newobj(node.Type.GetConstructor(new[] {node.Method.ReturnType}));
                            if(!operandIsNullLabelUsed)
                                context.EmitReturnDefaultValue(node.Type, valueIsNullLabel, il.DefineLabel("valueIsNotNull"));
                        }
                    }
                }
                else
                {
                    switch(node.NodeType)
                    {
                    case ExpressionType.Convert:
                        context.EmitConvert(resultType, node.Type); // stack: [(type)obj]
                        break;
                    case ExpressionType.ConvertChecked:
                        context.EmitConvert(resultType, node.Type, true); // stack: [(type)obj]
                        break;
                    default:
                        throw new InvalidOperationException("Node type '" + node.NodeType + "' is not valid at this point");
                    }
                }
                resultType = node.Type;
            }
            return false;
        }
    }
}