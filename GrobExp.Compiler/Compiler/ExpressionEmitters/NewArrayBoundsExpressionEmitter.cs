using System;
using System.Linq;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class NewArrayBoundsExpressionEmitter : ExpressionEmitter<NewArrayExpression>
    {
        protected override bool EmitInternal(NewArrayExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var il = context.Il;

            if(node.Expressions.Count != 1)
            {
                context.EmitLoadArguments(node.Expressions.ToArray());
                il.Newobj(node.Type.GetConstructor(node.Expressions.Select(exp => exp.Type).ToArray()));
            }
            else
            {
                GroboIL.Label lengthIsNullLabel = context.CanReturn ? il.DefineLabel("lengthIsNull") : null;
                Type lengthType;
                var labelUsed = ExpressionEmittersCollection.Emit(node.Expressions.Single(), context, lengthIsNullLabel, out lengthType);
                if(lengthType != typeof(int))
                    throw new InvalidOperationException("Cannot create an array with length of type '" + lengthType + "'");
                if(labelUsed && context.CanReturn)
                {
                    var lengthIsNotNullLabel = il.DefineLabel("lengthIsNotNull");
                    il.Br(lengthIsNotNullLabel);
                    context.MarkLabelAndSurroundWithSP(lengthIsNullLabel);
                    il.Pop();
                    il.Ldc_I4(0);
                    context.MarkLabelAndSurroundWithSP(lengthIsNotNullLabel);
                }
                il.Newarr(node.Type.GetElementType());
            }
            resultType = node.Type;
            return false;
        }
    }
}