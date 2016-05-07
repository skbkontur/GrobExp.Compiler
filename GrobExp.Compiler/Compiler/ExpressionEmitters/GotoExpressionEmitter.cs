using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class GotoExpressionEmitter : ExpressionEmitter<GotoExpression>
    {
        protected override bool EmitInternal(GotoExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            bool result = false;
            if(node.Value != null)
                result = ExpressionEmittersCollection.Emit(node.Value, context, returnDefaultValueLabel, out resultType);
            resultType = typeof(void);
            GroboIL.Label label;
            if(!context.Labels.TryGetValue(node.Target, out label))
                context.Labels.Add(node.Target, label = context.Il.DefineLabel(node.Target.Name));
            context.Il.Br(label);
            return result;
        }
    }
}