using System;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class NotExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        protected override bool EmitInternal(UnaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            if(node.Type != typeof(bool) && node.Type != typeof(bool?))
                return ExpressionEmittersCollection.Emit(Expression.OnesComplement(node.Operand, node.Method), context, returnDefaultValueLabel, whatReturn, extend, out resultType);
            GroboIL il = context.Il;
            if(node.Method != null)
                throw new NotSupportedException("Custom operator '" + node.NodeType + "' is not supported");
            var operand = node.Operand;

            context.EmitLoadArgument(operand, false, out resultType);
            if(resultType == typeof(bool))
            {
                il.Ldc_I4(1);
                il.Xor();
            }
            else if(resultType == typeof(bool?))
            {
                using(var value = context.DeclareLocal(typeof(bool?)))
                {
                    il.Stloc(value);
                    il.Ldloca(value);
                    context.EmitHasValueAccess(typeof(bool?));
                    var returnLabel = il.DefineLabel("return");
                    il.Brfalse(returnLabel);
                    il.Ldloca(value);
                    context.EmitValueAccess(typeof(bool?));
                    il.Ldc_I4(1);
                    il.Xor();
                    il.Newobj(nullableBoolConstructor);
                    il.Stloc(value);
                    context.MarkLabelAndSurroundWithSP(returnLabel);
                    il.Ldloc(value);
                }
            }
            else throw new InvalidOperationException("Cannot perform '" + node.NodeType + "' operator on type '" + resultType + "'");
            return false;
        }

        // ReSharper disable RedundantExplicitNullableCreation
        private static readonly ConstructorInfo nullableBoolConstructor = ((NewExpression)((Expression<Func<bool, bool?>>)(b => new bool?(b))).Body).Constructor;
        // ReSharper restore RedundantExplicitNullableCreation
    }
}