using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class TypeEqualExpressionEmitter : ExpressionEmitter<TypeBinaryExpression>
    {
        protected override bool EmitInternal(TypeBinaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            bool result;
            GroboIL il = context.Il;
            if(!node.Expression.Type.IsAssignableFrom(node.TypeOperand))
            {
                il.Ldc_I4(0);
                result = false;
            }
            else if(node.Expression.Type == node.TypeOperand && node.TypeOperand.IsValueType)
            {
                il.Ldc_I4(1);
                result = false;
            }
            else
            {
                Type operandType;
                result = ExpressionEmittersCollection.Emit(node.Expression, context, returnDefaultValueLabel, ResultType.Value, extend, out operandType);
                if(operandType.IsValueType)
                {
                    using(var temp = context.DeclareLocal(operandType))
                    {
                        il.Stloc(temp);
                        il.Ldloca(temp);
                    }
                }
                var returnFalseLabel = il.DefineLabel("returnFalse");
                il.Dup();
                il.Brfalse(returnFalseLabel);
                il.Call(typeof(object).GetMethod("GetType"), operandType);
                il.Ldtoken(node.TypeOperand);
                il.Call(typeof(Type).GetMethod("GetTypeFromHandle"));
                il.Ceq();
                var doneLabel = il.DefineLabel("done");
                il.Br(doneLabel);
                context.MarkLabelAndSurroundWithSP(returnFalseLabel);
                il.Pop();
                il.Ldc_I4(0);
                context.MarkLabelAndSurroundWithSP(doneLabel);
            }
            resultType = typeof(bool);
            return result;
        }
    }
}