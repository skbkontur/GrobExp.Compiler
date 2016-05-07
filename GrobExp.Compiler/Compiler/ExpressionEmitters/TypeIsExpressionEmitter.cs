using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class TypeIsExpressionEmitter : ExpressionEmitter<TypeBinaryExpression>
    {
        protected override bool EmitInternal(TypeBinaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            Type operandType;
            var result = ExpressionEmittersCollection.Emit(node.Expression, context, returnDefaultValueLabel, ResultType.Value, extend, out operandType);
            var il = context.Il;
            if(operandType.IsValueType)
                il.Box(operandType);
            il.Isinst(node.TypeOperand);
            il.Ldnull();
            il.Cgt(true);
            resultType = typeof(bool);
            return result;
        }
    }
}