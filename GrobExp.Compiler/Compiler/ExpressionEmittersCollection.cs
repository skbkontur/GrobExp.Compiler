using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrEmit;

using GrobExp.Compiler.ExpressionEmitters;

namespace GrobExp.Compiler
{
    internal static class ExpressionEmittersCollection
    {
        public static bool Emit(Expression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, out Type resultType)
        {
            return Emit(node, context, returnDefaultValueLabel, ResultType.Value, false, out resultType);
        }

        public static void Emit(Expression node, EmittingContext context, out Type resultType)
        {
            Emit(node, context, null, ResultType.Value, false, out resultType);
        }

        public static bool Emit(Expression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            IExpressionEmitter emitter;
            if(!expressionEmitters.TryGetValue(node.NodeType, out emitter))
                throw new NotSupportedException("Node type '" + node.NodeType + "' is not supported");
            return emitter.Emit(node, context, returnDefaultValueLabel, whatReturn, extend, out resultType);
        }

        private static readonly Dictionary<ExpressionType, IExpressionEmitter> expressionEmitters = new Dictionary<ExpressionType, IExpressionEmitter>
            {
                {ExpressionType.Parameter, new ParameterExpressionEmitter()},
                {ExpressionType.MemberAccess, new MemberAccessExpressionEmitter()},
                {ExpressionType.Convert, new ConvertExpressionEmitter()},
                {ExpressionType.ConvertChecked, new ConvertExpressionEmitter()},
                {ExpressionType.Constant, new ConstantExpressionEmitter()},
                {ExpressionType.ArrayIndex, new ArrayIndexExpressionEmitter()},
                {ExpressionType.Index, new IndexExpressionEmitter()},
                {ExpressionType.ArrayLength, new ArrayLengthExpressionEmitter()},
                {ExpressionType.Call, new CallExpressionEmitter()},
                {ExpressionType.Block, new BlockExpressionEmitter()},
                {ExpressionType.Assign, new AssignExpressionEmitter()},
                {ExpressionType.AddAssign, new AssignExpressionEmitter()},
                {ExpressionType.AddAssignChecked, new AssignExpressionEmitter()},
                {ExpressionType.SubtractAssign, new AssignExpressionEmitter()},
                {ExpressionType.SubtractAssignChecked, new AssignExpressionEmitter()},
                {ExpressionType.MultiplyAssign, new AssignExpressionEmitter()},
                {ExpressionType.MultiplyAssignChecked, new AssignExpressionEmitter()},
                {ExpressionType.DivideAssign, new AssignExpressionEmitter()},
                {ExpressionType.ModuloAssign, new AssignExpressionEmitter()},
                {ExpressionType.PowerAssign, new AssignExpressionEmitter()},
                {ExpressionType.AndAssign, new AssignExpressionEmitter()},
                {ExpressionType.OrAssign, new AssignExpressionEmitter()},
                {ExpressionType.ExclusiveOrAssign, new AssignExpressionEmitter()},
                {ExpressionType.LeftShiftAssign, new AssignExpressionEmitter()},
                {ExpressionType.RightShiftAssign, new AssignExpressionEmitter()},
                {ExpressionType.New, new NewExpressionEmitter()},
                {ExpressionType.Conditional, new ConditionalExpressionEmitter()},
                {ExpressionType.Equal, new EqualityExpressionEmitter()},
                {ExpressionType.NotEqual, new EqualityExpressionEmitter()},
                {ExpressionType.GreaterThan, new ComparisonExpressionEmitter()},
                {ExpressionType.LessThan, new ComparisonExpressionEmitter()},
                {ExpressionType.GreaterThanOrEqual, new ComparisonExpressionEmitter()},
                {ExpressionType.LessThanOrEqual, new ComparisonExpressionEmitter()},
                {ExpressionType.Add, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.AddChecked, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.Subtract, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.SubtractChecked, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.Multiply, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.MultiplyChecked, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.Divide, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.Modulo, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.Power, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.LeftShift, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.RightShift, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.And, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.Or, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.ExclusiveOr, new BinaryArithmeticOperationExpressionEmitter()},
                {ExpressionType.OrElse, new LogicalExpressionEmitter()},
                {ExpressionType.AndAlso, new LogicalExpressionEmitter()},
                {ExpressionType.Not, new NotExpressionEmitter()},
                {ExpressionType.MemberInit, new MemberInitExpressionEmitter()},
                {ExpressionType.NewArrayInit, new NewArrayInitExpressionEmitter()},
                {ExpressionType.NewArrayBounds, new NewArrayBoundsExpressionEmitter()},
                {ExpressionType.Default, new DefaultExpressionEmitter()},
                {ExpressionType.Coalesce, new CoalesceExpressionEmitter()},
                {ExpressionType.Lambda, new LambdaExpressionEmitter()},
                {ExpressionType.Label, new LabelExpressionEmitter()},
                {ExpressionType.Goto, new GotoExpressionEmitter()},
                {ExpressionType.UnaryPlus, new UnaryAritmeticOperationExpressionEmitter()},
                {ExpressionType.Negate, new UnaryAritmeticOperationExpressionEmitter()},
                {ExpressionType.NegateChecked, new UnaryAritmeticOperationExpressionEmitter()},
                {ExpressionType.Increment, new UnaryAritmeticOperationExpressionEmitter()},
                {ExpressionType.Decrement, new UnaryAritmeticOperationExpressionEmitter()},
                {ExpressionType.OnesComplement, new UnaryAritmeticOperationExpressionEmitter()},
                {ExpressionType.IsTrue, new UnaryAritmeticOperationExpressionEmitter()},
                {ExpressionType.IsFalse, new UnaryAritmeticOperationExpressionEmitter()},
                {ExpressionType.PreIncrementAssign, new UnaryAssignExpressionEmitter()},
                {ExpressionType.PreDecrementAssign, new UnaryAssignExpressionEmitter()},
                {ExpressionType.PostIncrementAssign, new UnaryAssignExpressionEmitter()},
                {ExpressionType.PostDecrementAssign, new UnaryAssignExpressionEmitter()},
                {ExpressionType.TypeIs, new TypeIsExpressionEmitter()},
                {ExpressionType.TypeEqual, new TypeEqualExpressionEmitter()},
                {ExpressionType.TypeAs, new TypeAsExpressionEmitter()},
                {ExpressionType.Unbox, new UnboxExpressionEmitter()},
                {ExpressionType.Invoke, new InvocationExpressionEmitter()},
                {ExpressionType.Throw, new ThrowExpressionEmitter()},
                {ExpressionType.ListInit, new ListInitExpressionEmitter()},
                {ExpressionType.Loop, new LoopExpressionEmitter()},
                {ExpressionType.Try, new TryExpressionEmitter()},
                {ExpressionType.Switch, new SwitchExpressionEmitter()},
                {ExpressionType.DebugInfo, new DebugInfoExpressionEmitter()},
            };
    }
}