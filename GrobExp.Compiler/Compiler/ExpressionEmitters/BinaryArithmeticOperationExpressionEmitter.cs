using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class BinaryArithmeticOperationExpressionEmitter : ExpressionEmitter<BinaryExpression>
    {
        protected override bool EmitInternal(BinaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            Expression left = node.Left;
            Expression right = node.Right;
            Type leftType, rightType;
            context.EmitLoadArgument(left, false, out leftType);
            context.EmitLoadArgument(right, false, out rightType);
            context.EmitArithmeticOperation(node.NodeType, node.Type, leftType, rightType, node.Method);
            resultType = node.Type;
            return false;
        }
    }
}