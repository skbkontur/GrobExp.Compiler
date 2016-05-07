using System.Collections.Generic;
using System.Linq.Expressions;

namespace GrobExp.Compiler
{
    internal class LabelsCloner : ExpressionVisitor
    {
        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            if(node == null)
                return null;
            LabelTarget newLabel;
            if(!labels.TryGetValue(node, out newLabel))
            {
                newLabel = Expression.Label(node.Type, node.Name);
                labels.Add(node, newLabel);
            }
            return newLabel;
        }

        private readonly Dictionary<LabelTarget, LabelTarget> labels = new Dictionary<LabelTarget, LabelTarget>();
    }
}