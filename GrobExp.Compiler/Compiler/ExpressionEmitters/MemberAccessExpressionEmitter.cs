using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class MemberAccessExpressionEmitter : ExpressionEmitter<MemberExpression>
    {
        protected override bool EmitInternal(MemberExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            EmittingContext.LocalHolder owner;
            var result = context.EmitMemberAccess(node, returnDefaultValueLabel, context.Options.HasFlag(CompilerOptions.CheckNullReferences), extend, whatReturn, out resultType, out owner);
            if(owner != null)
                owner.Dispose();
            return result;
        }
    }
}