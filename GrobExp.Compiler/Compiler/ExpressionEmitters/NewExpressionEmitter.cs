using System;
using System.Linq;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class NewExpressionEmitter : ExpressionEmitter<NewExpression>
    {
        protected override bool EmitInternal(NewExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            context.EmitLoadArguments(node.Arguments.ToArray());
            // note ich: баг решарпера
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            GroboIL il = context.Il;
            if(node.Constructor != null)
                il.Newobj(node.Constructor);
            else
            {
                if(node.Type.IsValueType)
                {
                    using(var temp = context.DeclareLocal(node.Type))
                    {
                        il.Ldloca(temp);
                        il.Initobj(node.Type);
                        il.Ldloc(temp);
                    }
                }
                else throw new InvalidOperationException("Missing constructor for type '" + node.Type + "'");
            }
            resultType = node.Type;
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            // ReSharper restore HeuristicUnreachableCode
            return false;
        }
    }
}