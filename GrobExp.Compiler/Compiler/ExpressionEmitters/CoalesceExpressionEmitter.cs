using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class CoalesceExpressionEmitter : ExpressionEmitter<BinaryExpression>
    {
        protected override bool EmitInternal(BinaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            if(node.Conversion != null)
                throw new NotSupportedException("Coalesce with conversion is not supported");
            // note ich: баг решарпера
            // ReSharper disable HeuristicUnreachableCode
            var left = node.Left;
            var right = node.Right;
            GroboIL il = context.Il;
            GroboIL.Label valueIsNullLabel = il.DefineLabel("valueIsNull");
            Type leftType;
            bool labelUsed = ExpressionEmittersCollection.Emit(left, context, valueIsNullLabel, out leftType);
            if(left.Type.IsValueType)
            {
                using(var temp = context.DeclareLocal(left.Type))
                {
                    il.Stloc(temp);
                    il.Ldloca(temp);
                }
            }
            labelUsed |= context.EmitNullChecking(left.Type, valueIsNullLabel);
            if(left.Type.IsValueType)
            {
                if(!left.Type.IsNullable())
                    throw new InvalidOperationException("Type '" + left.Type + "' cannot be null");
                if(node.Type != left.Type)
                    context.EmitValueAccess(left.Type);
                else
                    il.Ldobj(left.Type);
            }
            var valueIsNotNullLabel = il.DefineLabel("valueIsNotNull");
            il.Br(valueIsNotNullLabel);
            if(labelUsed)
            {
                context.MarkLabelAndSurroundWithSP(valueIsNullLabel);
                il.Pop();
            }
            Type rightType;
            var result = ExpressionEmittersCollection.Emit(right, context, returnDefaultValueLabel, out rightType);
            context.MarkLabelAndSurroundWithSP(valueIsNotNullLabel);
            resultType = node.Type;
            return result;
            // ReSharper restore HeuristicUnreachableCode
        }
    }
}