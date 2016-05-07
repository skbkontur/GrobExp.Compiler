using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class ArrayLengthExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        protected override bool EmitInternal(UnaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var il = context.Il;
            Type arrayType;
            var result = ExpressionEmittersCollection.Emit(node.Operand, context, returnDefaultValueLabel, out arrayType);
            if(!arrayType.IsArray)
                throw new InvalidOperationException("Expected an array but was '" + arrayType + "'");
            if(context.Options.HasFlag(CompilerOptions.CheckNullReferences))
            {
                result = true;
                il.Dup();
                il.Brfalse(returnDefaultValueLabel);
            }
            il.Ldlen();
            resultType = typeof(int);
            return result;
        }
    }
}