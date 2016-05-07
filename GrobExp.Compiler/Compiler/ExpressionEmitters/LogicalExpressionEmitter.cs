using System;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class LogicalExpressionEmitter : ExpressionEmitter<BinaryExpression>
    {
        protected override bool EmitInternal(BinaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            GroboIL il = context.Il;
            if(node.Method != null)
                throw new NotSupportedException("Custom operator '" + node.NodeType + "' is not supported");
            Expression left = node.Left;
            Expression right = node.Right;
            Type leftType;
            context.EmitLoadArgument(left, false, out leftType); // stack: [left]
            if(leftType == typeof(bool))
            {
                switch(node.NodeType)
                {
                case ExpressionType.AndAlso:
                    {
                        var returnFalseLabel = il.DefineLabel("returnFalse");
                        il.Brfalse(returnFalseLabel); // if(left == false) return false; stack: []
                        Type rightType;
                        context.EmitLoadArgument(right, false, out rightType); // stack: [right]
                        var doneLabel = il.DefineLabel("done");
                        il.Br(doneLabel); // goto done; stack: [right]
                        context.MarkLabelAndSurroundWithSP(returnFalseLabel);
                        il.Ldc_I4(0); // stack: [false]
                        if(rightType == typeof(bool?))
                            il.Newobj(nullableBoolConstructor); // stack: [new bool?(false)]
                        context.MarkLabelAndSurroundWithSP(doneLabel);
                        resultType = rightType;
                        break;
                    }
                case ExpressionType.OrElse:
                    {
                        var returnTrueLabel = il.DefineLabel("returnTrue");
                        il.Brtrue(returnTrueLabel); // if(left == true) return true; stack: []
                        Type rightType;
                        context.EmitLoadArgument(right, false, out rightType); // stack: [right]
                        var doneLabel = il.DefineLabel("done");
                        il.Br(doneLabel); // goto done; stack: [right]
                        context.MarkLabelAndSurroundWithSP(returnTrueLabel);
                        il.Ldc_I4(1); // stack: [true]
                        if(rightType == typeof(bool?))
                            il.Newobj(nullableBoolConstructor); // stack: [new bool?(true)]
                        context.MarkLabelAndSurroundWithSP(doneLabel);
                        resultType = rightType;
                        break;
                    }
                default:
                    throw new NotSupportedException("Node type '" + node.NodeType + "' is not supported");
                }
            }
            else // bool?
            {
                switch(node.NodeType)
                {
                case ExpressionType.AndAlso:
                    {
                        /*
                                         * +-------+-------+-------+-------+
                                         * |  &&   | null  | false | true  |
                                         * +-------+-------+-------+-------+
                                         * | null  | null  | false | null  |
                                         * +-------+-------+-------+-------+
                                         * | false | false | false | false |
                                         * +-------+-------+-------+-------+
                                         * | true  | null  | false | true  |
                                         * +-------+-------+-------+-------+
                                     */
                        using(var localLeft = context.DeclareLocal(typeof(bool?)))
                        {
                            il.Stloc(localLeft); // localLeft = left; stack: []
                            il.Ldloca(localLeft); // stack: [&localLeft]
                            context.EmitHasValueAccess(typeof(bool?)); // stack: [localLeft.HasValue]
                            il.Ldc_I4(1); // stack: [localLeft.HasValue, true]
                            il.Xor(); // stack: [!localLeft.HasValue]
                            il.Ldloca(localLeft); // stack: [!localLeft.HasValue, &localLeft]
                            context.EmitValueAccess(typeof(bool?)); // stack: [!localLeft.HasValue, localLeft.Value]
                            il.Or(); // stack: [!localLeft.HasValue || localLeft.Value]
                            var returnFalseLabel = il.DefineLabel("returnFalse");
                            il.Brfalse(returnFalseLabel); // if(localLeft == false) goto returnFalse; stack: []
                            Type rightType;
                            context.EmitLoadArgument(right, false, out rightType); // stack: [right]
                            using(var localRight = context.DeclareLocal(rightType))
                            {
                                il.Stloc(localRight); // localRight = right; stack: []
                                if(rightType == typeof(bool))
                                    il.Ldloc(localRight); // stack: [localRight]
                                else
                                {
                                    il.Ldloca(localRight); // stack: [&localRight]
                                    context.EmitHasValueAccess(typeof(bool?)); // stack: [localRight.HasValue]
                                    il.Ldc_I4(1); // stack: [localRight.HasValue, true]
                                    il.Xor(); // stack: [!localRight.HasValue]
                                    il.Ldloca(localRight); // stack: [!localRight.HasValue, &localRight]
                                    context.EmitValueAccess(typeof(bool?)); // stack: [!localRight.HasValue, localRight.Value]
                                    il.Or(); // stack: [!localRight.HasValue || localRight.Value]
                                }
                                il.Brfalse(returnFalseLabel); // if(localRight == false) goto returnFalse;
                                if(rightType == typeof(bool))
                                    il.Ldloc(localRight); // stack: [localRight]
                                else
                                {
                                    il.Ldloca(localRight); // stack: [&localRight]
                                    context.EmitHasValueAccess(typeof(bool?)); // stack: [localRight.HasValue]
                                    il.Ldloca(localRight); // stack: [localRight.HasValue, &localRight]
                                    context.EmitValueAccess(typeof(bool?)); // stack: [localRight.HasValue, localRight.Value]
                                    il.And(); // stack: [localRight.HasValue && localRight.Value]
                                }
                                var returnLeftLabel = il.DefineLabel("returnLeft");
                                il.Brtrue(returnLeftLabel); // if(localRight == true) goto returnLeft;
                                il.Ldloca(localLeft); // stack: [&localLeft]
                                il.Initobj(typeof(bool?)); // localLeft = default(bool?); stack: []
                                context.MarkLabelAndSurroundWithSP(returnLeftLabel);
                                il.Ldloc(localLeft); // stack: [localLeft]
                                var doneLabel = il.DefineLabel("done");
                                il.Br(doneLabel);
                                context.MarkLabelAndSurroundWithSP(returnFalseLabel);
                                il.Ldc_I4(0); // stack: [false]
                                il.Newobj(nullableBoolConstructor); // new bool?(false)
                                context.MarkLabelAndSurroundWithSP(doneLabel);
                                resultType = typeof(bool?);
                            }
                        }
                        break;
                    }
                case ExpressionType.OrElse:
                    {
                        /*
                                         * +-------+-------+-------+-------+
                                         * |  ||   | null  | false | true  |
                                         * +-------+-------+-------+-------+
                                         * | null  | null  | null  | true  |
                                         * +-------+-------+-------+-------+
                                         * | false | null  | false | true  |
                                         * +-------+-------+-------+-------+
                                         * | true  | true  | true  | true  |
                                         * +-------+-------+-------+-------+
                                     */
                        using(var localLeft = context.DeclareLocal(typeof(bool?)))
                        {
                            il.Stloc(localLeft); // localLeft = left; stack: []
                            il.Ldloca(localLeft); // stack: [&localLeft]
                            context.EmitHasValueAccess(typeof(bool?)); // stack: [localLeft.HasValue]
                            il.Ldloca(localLeft); // stack: [localLeft.HasValue, &localLeft]
                            context.EmitValueAccess(typeof(bool?)); // stack: [localLeft.HasValue, localLeft.Value]
                            il.And(); // stack: [localLeft.HasValue && localLeft.Value]
                            var returnTrueLabel = il.DefineLabel("returnTrue");
                            il.Brtrue(returnTrueLabel); // if(localLeft == true) goto returnTrue; stack: []
                            Type rightType;
                            context.EmitLoadArgument(right, false, out rightType); // stack: [right]
                            using(var localRight = context.DeclareLocal(rightType))
                            {
                                il.Stloc(localRight); // localRight = right; stack: []
                                if(rightType == typeof(bool))
                                    il.Ldloc(localRight); // stack: [localRight]
                                else
                                {
                                    il.Ldloca(localRight); // stack: [&localRight]
                                    context.EmitHasValueAccess(typeof(bool?)); // stack: [localRight.HasValue]
                                    il.Ldloca(localRight); // stack: [localRight.HasValue, &localRight]
                                    context.EmitValueAccess(typeof(bool?)); // stack: [localRight.HasValue, localRight.Value]
                                    il.And(); // stack: [localRight.HasValue && localRight.Value]
                                }
                                il.Brtrue(returnTrueLabel); // if(localRight == true) goto returnTrue; stack: []
                                if(rightType == typeof(bool))
                                    il.Ldloc(localRight); // stack: [localRight]
                                else
                                {
                                    il.Ldloca(localRight); // stack: [&localRight]
                                    context.EmitHasValueAccess(typeof(bool?)); // stack: [localRight.HasValue]
                                    il.Ldc_I4(1); // stack: [localRight.HasValue, true]
                                    il.Xor(); // stack: [!localRight.HasValue]
                                    il.Ldloca(localRight); // stack: [!localRight.HasValue, &localRight]
                                    context.EmitValueAccess(typeof(bool?)); // stack: [!localRight.HasValue, localRight.Value]
                                    il.Or(); // stack: [!localRight.HasValue || localRight.Value]
                                }
                                var returnLeftLabel = il.DefineLabel("returnLeft");
                                il.Brfalse(returnLeftLabel); // if(localRight == false) goto returnLeft;
                                il.Ldloca(localLeft); // stack: [&localLeft]
                                il.Initobj(typeof(bool?)); // localLeft = default(bool?); stack: []
                                context.MarkLabelAndSurroundWithSP(returnLeftLabel);
                                il.Ldloc(localLeft); // stack: [localLeft]
                                var doneLabel = il.DefineLabel("done");
                                il.Br(doneLabel);
                                context.MarkLabelAndSurroundWithSP(returnTrueLabel);
                                il.Ldc_I4(1); // stack: [true]
                                il.Newobj(nullableBoolConstructor); // new bool?(true)
                                context.MarkLabelAndSurroundWithSP(doneLabel);
                                resultType = typeof(bool?);
                            }
                        }
                        break;
                    }
                default:
                    throw new NotSupportedException("Node type '" + node.NodeType + "' is not supported");
                }
            }
            return false;
        }

        // ReSharper disable RedundantExplicitNullableCreation
        private static readonly ConstructorInfo nullableBoolConstructor = ((NewExpression)((Expression<Func<bool, bool?>>)(b => new bool?(b))).Body).Constructor;
        // ReSharper restore RedundantExplicitNullableCreation
    }
}