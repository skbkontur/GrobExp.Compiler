using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class EqualityExpressionEmitter : ExpressionEmitter<BinaryExpression>
    {
        protected override bool EmitInternal(BinaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            Expression left = node.Left;
            Expression right = node.Right;
            Type leftType, rightType;
            context.EmitLoadArgument(left, false, out leftType);
            context.EmitLoadArgument(right, false, out rightType);
            GroboIL il = context.Il;
            if(!leftType.IsNullable() && !rightType.IsNullable())
            {
                if(node.Method != null)
                    il.Call(node.Method);
                else
                {
                    if(leftType.IsStruct() || rightType.IsStruct())
                        throw new InvalidOperationException("Cannot compare structs");
                    il.Ceq();
                    if(node.NodeType == ExpressionType.NotEqual)
                    {
                        il.Ldc_I4(1);
                        il.Xor();
                    }
                }
            }
            else
            {
                var type = leftType;
                if(type != rightType)
                    throw new InvalidOperationException("Cannot compare objects of different types '" + leftType + "' and '" + rightType + "'");
                using(var localLeft = context.DeclareLocal(type))
                using(var localRight = context.DeclareLocal(type))
                {
                    il.Stloc(localRight);
                    il.Stloc(localLeft);
                    if(node.Method != null)
                    {
                        il.Ldloca(localLeft); // stack: [&left]
                        context.EmitHasValueAccess(type); // stack: [left.HasValue]
                        il.Dup(); // stack: [left.HasValue, left.HasValue]
                        il.Ldloca(localRight); // stack: [left.HasValue, left.HasValue, &right]
                        context.EmitHasValueAccess(type); // stack: [left.HasValue, left.HasValue, right.HasValue]
                        var notEqualLabel = il.DefineLabel("notEqual");
                        il.Bne_Un(notEqualLabel); // stack: [left.HasValue]
                        var equalLabel = il.DefineLabel("equal");
                        il.Brfalse(equalLabel);
                        il.Ldloca(localLeft);
                        context.EmitValueAccess(type);
                        il.Ldloca(localRight);
                        context.EmitValueAccess(type);
                        il.Call(node.Method);
                        var doneLabel = il.DefineLabel("done");
                        il.Br(doneLabel);
                        context.MarkLabelAndSurroundWithSP(notEqualLabel);
                        il.Pop();
                        il.Ldc_I4(node.NodeType == ExpressionType.Equal ? 0 : 1);
                        il.Br(doneLabel);
                        context.MarkLabelAndSurroundWithSP(equalLabel);
                        il.Ldc_I4(node.NodeType == ExpressionType.Equal ? 1 : 0);
                        context.MarkLabelAndSurroundWithSP(doneLabel);
                    }
                    else
                    {
                        il.Ldloca(localLeft);
                        context.EmitValueAccess(type);
                        il.Ldloca(localRight);
                        context.EmitValueAccess(type);
                        var notEqualLabel = il.DefineLabel("notEqual");
                        il.Bne_Un(notEqualLabel);
                        il.Ldloca(localLeft);
                        context.EmitHasValueAccess(type);
                        il.Ldloca(localRight);
                        context.EmitHasValueAccess(type);
                        il.Ceq();
                        var doneLabel = il.DefineLabel("done");
                        il.Br(doneLabel);
                        context.MarkLabelAndSurroundWithSP(notEqualLabel);
                        il.Ldc_I4(0);
                        context.MarkLabelAndSurroundWithSP(doneLabel);
                        if(node.NodeType == ExpressionType.NotEqual)
                        {
                            il.Ldc_I4(1);
                            il.Xor();
                        }
                    }
                }
            }
            resultType = typeof(bool);
            return false;
        }
    }
}