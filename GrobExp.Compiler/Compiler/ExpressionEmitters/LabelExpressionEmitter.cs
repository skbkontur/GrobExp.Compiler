using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class LabelExpressionEmitter : ExpressionEmitter<LabelExpression>
    {
        protected override bool EmitInternal(LabelExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            bool result = false;
            if(node.DefaultValue != null)
                result = ExpressionEmittersCollection.Emit(node.DefaultValue, context, returnDefaultValueLabel, out resultType);
            else
                resultType = typeof(void);
            GroboIL.Label label;
            if(!context.Labels.TryGetValue(node.Target, out label))
                context.Labels.Add(node.Target, label = context.Il.DefineLabel(node.Target.Name));
            context.MarkLabelAndSurroundWithSP(label);
            return result;
        }
    }
}