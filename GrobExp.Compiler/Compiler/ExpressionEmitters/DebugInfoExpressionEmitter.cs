using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class DebugInfoExpressionEmitter : ExpressionEmitter<Expression>
    {
        protected override bool EmitInternal(Expression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            resultType = typeof(void);
            if(context.DebugInfoGenerator == null)
                return false;
            var result = false;
            DebugInfoExpression debugInfo;
            if(!(node is TypedDebugInfoExpression))
                debugInfo = (DebugInfoExpression)node;
            else
            {
                var typedNode = node as TypedDebugInfoExpression;
                result = ExpressionEmittersCollection.Emit(typedNode.Expression, context, returnDefaultValueLabel, out resultType);
                debugInfo = typedNode.DebugInfo;
            }
            markSequencePoint(context.DebugInfoGenerator, context.Lambda, context.Method, context.Il, debugInfo);
            context.Il.Nop();
            return result;
        }

        private static Action<DebugInfoGenerator, LambdaExpression, MethodBase, GroboIL, DebugInfoExpression> BuildSequencePointMarker()
        {
            var parameterTypes = new[] {typeof(DebugInfoGenerator), typeof(LambdaExpression), typeof(MethodBase), typeof(GroboIL), typeof(DebugInfoExpression)};
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), typeof(void), parameterTypes, typeof(DebugInfoExpressionEmitter), true);
            using(var il = new GroboIL(dynamicMethod))
            {
                il.Ldarg(0);
                il.Ldarg(1);
                il.Ldarg(2);
                il.Ldarg(3);
                il.Ldfld(typeof(GroboIL).GetField("il", BindingFlags.NonPublic | BindingFlags.Instance));
                il.Ldarg(4);
                var markSequencePointMethod = typeof(DebugInfoGenerator).GetMethod("MarkSequencePoint", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof(LambdaExpression), typeof(MethodBase), typeof(ILGenerator), typeof(DebugInfoExpression)}, null);
                il.Call(markSequencePointMethod, typeof(DebugInfoGenerator));
                il.Ret();
            }
            return (Action<DebugInfoGenerator, LambdaExpression, MethodBase, GroboIL, DebugInfoExpression>)dynamicMethod.CreateDelegate(typeof(Action<DebugInfoGenerator, LambdaExpression, MethodBase, GroboIL, DebugInfoExpression>));
        }

        private static readonly Action<DebugInfoGenerator, LambdaExpression, MethodInfo, GroboIL, DebugInfoExpression> markSequencePoint = BuildSequencePointMarker();
    }
}