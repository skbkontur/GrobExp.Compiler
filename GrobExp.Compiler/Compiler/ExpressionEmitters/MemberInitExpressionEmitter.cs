using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class MemberInitExpressionEmitter : ExpressionEmitter<MemberInitExpression>
    {
        protected override bool EmitInternal(MemberInitExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            ExpressionEmittersCollection.Emit(node.NewExpression, context, out resultType); // stack: [new obj(args)]
            GroboIL il = context.Il;
            if(!node.Type.IsValueType)
            {
                foreach(MemberAssignment assignment in node.Bindings)
                {
                    il.Dup();
                    context.EmitLoadArguments(assignment.Expression);
                    context.EmitMemberAssign(node.Type, assignment.Member);
                }
            }
            else
            {
                using(var temp = context.DeclareLocal(node.Type))
                {
                    il.Stloc(temp);
                    il.Ldloca(temp);
                    foreach(MemberAssignment assignment in node.Bindings)
                    {
                        il.Dup();
                        context.EmitLoadArguments(assignment.Expression);
                        context.EmitMemberAssign(node.Type, assignment.Member);
                    }
                    // todo или il.Ldobj()?
                    il.Pop();
                    il.Ldloc(temp);
                }
            }
            return false;
        }
    }
}