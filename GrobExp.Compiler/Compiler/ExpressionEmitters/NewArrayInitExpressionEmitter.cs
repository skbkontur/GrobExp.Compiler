using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class NewArrayInitExpressionEmitter : ExpressionEmitter<NewArrayExpression>
    {
        protected override bool EmitInternal(NewArrayExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var il = context.Il;
            Type elementType = node.Type.GetElementType();
            il.Ldc_I4(node.Expressions.Count); // stack: [length]
            il.Newarr(elementType); // stack: [new type[length]]
            for(int index = 0; index < node.Expressions.Count; index++)
            {
                var expression = node.Expressions[index];
                il.Dup();
                il.Ldc_I4(index);
                if(elementType.IsValueType && !elementType.IsPrimitive)
                {
                    il.Ldelema(elementType);
                    context.EmitLoadArguments(expression);
                    il.Stobj(elementType);
                }
                else
                {
                    context.EmitLoadArguments(expression);
                    il.Stelem(elementType);
                }
            }
            resultType = node.Type;
            return false;
        }
    }
}