using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class TypeAsExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        protected override bool EmitInternal(UnaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            Type operandType;
            var result = ExpressionEmittersCollection.Emit(node.Operand, context, returnDefaultValueLabel, ResultType.Value, extend, out operandType);
            GroboIL il = context.Il;
            if(operandType.IsValueType)
                il.Box(operandType);
            il.Isinst(node.Type);
            if(node.Type.IsValueType)
                il.Unbox_Any(node.Type);
            resultType = node.Type;
            return result;
        }
    }
}