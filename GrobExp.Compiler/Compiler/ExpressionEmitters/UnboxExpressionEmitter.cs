using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class UnboxExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        protected override bool EmitInternal(UnaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            Type operandType;
            var result = ExpressionEmittersCollection.Emit(node.Operand, context, returnDefaultValueLabel, ResultType.Value, extend, out operandType);
            if(context.Options.HasFlag(CompilerOptions.CheckNullReferences))
            {
                context.Il.Dup();
                context.Il.Brfalse(returnDefaultValueLabel);
                result = true;
            }
            context.Il.Unbox_Any(node.Type);
            resultType = node.Type;
            return result;
        }
    }
}