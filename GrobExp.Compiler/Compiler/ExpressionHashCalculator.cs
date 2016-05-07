using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GrobExp.Compiler
{
    public static class ExpressionHashCalculator
    {
        public static int CalcHashCode(Expression node, bool strictly)
        {
            var hashCodes = new List<int>();
            CalcHashCode(node, new Context
            {
                Strictly = strictly,
                Parameters = new Dictionary<Type, Dictionary<ParameterExpression, int>>(),
                Labels = new Dictionary<LabelTarget, int>(),
                HashCodes = hashCodes
            });
            const int x = 1084996987; //for sake of primality
            var result = 0;
            foreach(var hashCode in hashCodes)
            {
                unchecked
                {
                    result = result * x + hashCode;
                }
            }
            return result;
        }

        private static void CalcHashCode(IEnumerable<Expression> list, Context context)
        {
            foreach(var exp in list)
                CalcHashCode(exp, context);
        }

        private static void CalcHashCode(Expression node, Context context)
        {
            if(node == null)
            {
                context.HashCodes.Add(0);
                return;
            }
            context.HashCodes.Add(CalcHashCode(node.NodeType));
            context.HashCodes.Add(CalcHashCode(node.Type));
            switch(node.NodeType)
            {
            case ExpressionType.Add:
            case ExpressionType.AddAssign:
            case ExpressionType.AddAssignChecked:
            case ExpressionType.AddChecked:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
            case ExpressionType.AndAssign:
            case ExpressionType.ArrayIndex:
            case ExpressionType.Assign:
            case ExpressionType.Coalesce:
            case ExpressionType.Divide:
            case ExpressionType.DivideAssign:
            case ExpressionType.Equal:
            case ExpressionType.ExclusiveOr:
            case ExpressionType.ExclusiveOrAssign:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LeftShift:
            case ExpressionType.LeftShiftAssign:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.Modulo:
            case ExpressionType.ModuloAssign:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyAssign:
            case ExpressionType.MultiplyAssignChecked:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.NotEqual:
            case ExpressionType.Or:
            case ExpressionType.OrAssign:
            case ExpressionType.OrElse:
            case ExpressionType.Power:
            case ExpressionType.PowerAssign:
            case ExpressionType.RightShift:
            case ExpressionType.RightShiftAssign:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractAssign:
            case ExpressionType.SubtractAssignChecked:
            case ExpressionType.SubtractChecked:
                CalcHashCodeBinary((BinaryExpression)node, context);
                break;
            case ExpressionType.ArrayLength:
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
            case ExpressionType.Decrement:
            case ExpressionType.Increment:
            case ExpressionType.IsFalse:
            case ExpressionType.IsTrue:
            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
            case ExpressionType.Not:
            case ExpressionType.OnesComplement:
            case ExpressionType.PostDecrementAssign:
            case ExpressionType.PostIncrementAssign:
            case ExpressionType.PreDecrementAssign:
            case ExpressionType.PreIncrementAssign:
            case ExpressionType.TypeAs:
            case ExpressionType.UnaryPlus:
            case ExpressionType.Unbox:
            case ExpressionType.Quote:
            case ExpressionType.Throw:
                CalcHashCodeUnary((UnaryExpression)node, context);
                break;
            case ExpressionType.Parameter:
                CalcHashCodeParameter((ParameterExpression)node, context);
                break;
            case ExpressionType.Block:
                CalcHashCodeBlock((BlockExpression)node, context);
                break;
            case ExpressionType.Call:
                CalcHashCodeCall((MethodCallExpression)node, context);
                break;
            case ExpressionType.Conditional:
                CalcHashCodeConditional((ConditionalExpression)node, context);
                break;
            case ExpressionType.Constant:
                CalcHashCodeConstant((ConstantExpression)node, context);
                break;
            case ExpressionType.DebugInfo:
                CalcHashCodeDebugInfo((DebugInfoExpression)node, context);
                break;
            case ExpressionType.Default:
                CalcHashCodeDefault((DefaultExpression)node, context);
                break;
            case ExpressionType.Dynamic:
                CalcHashCodeDynamic((DynamicExpression)node, context);
                break;
            case ExpressionType.Extension:
                CalcHashCodeExtension(node, context);
                break;
            case ExpressionType.Goto:
                CalcHashCodeGoto((GotoExpression)node, context);
                break;
            case ExpressionType.Index:
                CalcHashCodeIndex((IndexExpression)node, context);
                break;
            case ExpressionType.Invoke:
                CalcHashCodeInvoke((InvocationExpression)node, context);
                break;
            case ExpressionType.Label:
                CalcHashCodeLabel((LabelExpression)node, context);
                break;
            case ExpressionType.Lambda:
                CalcHashCodeLambda((LambdaExpression)node, context);
                break;
            case ExpressionType.ListInit:
                CalcHashCodeListInit((ListInitExpression)node, context);
                break;
            case ExpressionType.Loop:
                CalcHashCodeLoop((LoopExpression)node, context);
                break;
            case ExpressionType.MemberAccess:
                CalcHashCodeMemberAccess((MemberExpression)node, context);
                break;
            case ExpressionType.MemberInit:
                CalcHashCodeMemberInit((MemberInitExpression)node, context);
                break;
            case ExpressionType.New:
                CalcHashCodeNew((NewExpression)node, context);
                break;
            case ExpressionType.NewArrayBounds:
            case ExpressionType.NewArrayInit:
                CalcHashCodeNewArray((NewArrayExpression)node, context);
                break;
            case ExpressionType.RuntimeVariables:
                CalcHashCodeRuntimeVariables((RuntimeVariablesExpression)node, context);
                break;
            case ExpressionType.Switch:
                CalcHashCodeSwitch((SwitchExpression)node, context);
                break;
            case ExpressionType.Try:
                CalcHashCodeTry((TryExpression)node, context);
                break;
            case ExpressionType.TypeEqual:
            case ExpressionType.TypeIs:
                CalcHashCodeTypeBinary((TypeBinaryExpression)node, context);
                break;
            default:
                throw new NotSupportedException("Node type '" + node.NodeType + "' is not supported");
            }
        }

        private static int CalcHashCode(ExpressionType expressionType)
        {
            return (int)expressionType;
        }

        private static int CalcHashCode(MemberBindingType bindingType)
        {
            return (int)bindingType;
        }

        private static int CalcHashCode(MemberInfo member)
        {
            return member == null ? 0 : unchecked(member.Module.MetadataToken * 397 + member.MetadataToken);
        }

        private static int CalcHashCode(string str)
        {
            return str == null ? 0 : str.GetHashCode();
        }

        private static int CalcHashCode(int x)
        {
            return x;
        }

        private static int CalcHashCode(Type type)
        {
            return unchecked(type.Module.MetadataToken * 397 + type.MetadataToken);
        }

        private static int CalcHashCode(GotoExpressionKind kind)
        {
            return (int)kind;
        }

        private static int CalcHashCodeObject(object obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }

        private static void CalcHashCodeParameter(ParameterExpression node, Context context)
        {
            if(node == null)
            {
                context.HashCodes.Add(0);
                return;
            }

            var parameterType = node.IsByRef ? node.Type.MakeByRefType() : node.Type;
            context.HashCodes.Add(CalcHashCode(parameterType));
            if(context.Strictly)
                context.HashCodes.Add(CalcHashCode(node.Name));
            else
            {
                Dictionary<ParameterExpression, int> parameters;
                if(!context.Parameters.TryGetValue(parameterType, out parameters))
                    context.Parameters.Add(parameterType, parameters = new Dictionary<ParameterExpression, int>());
                int index;
                if(!parameters.TryGetValue(node, out index))
                    parameters.Add(node, index = parameters.Count);
                context.HashCodes.Add(CalcHashCode(index));
            }
        }

        private static void CalcHashCodeUnary(UnaryExpression node, Context context)
        {
            context.HashCodes.Add(CalcHashCode(node.Method));
            CalcHashCode(node.Operand, context);
        }

        private static void CalcHashCodeBinary(BinaryExpression node, Context context)
        {
            context.HashCodes.Add(CalcHashCode(node.Method));
            CalcHashCode(node.Left, context);
            CalcHashCode(node.Right, context);
        }

        private static void CalcHashCodeBlock(BlockExpression node, Context context)
        {
            if(context.Strictly)
            {
                foreach(var variable in node.Variables)
                    CalcHashCodeParameter(variable, context);
            }

            if(!context.Strictly)
            {
                foreach(var variable in node.Variables)
                {
                    Dictionary<ParameterExpression, int> parameters;
                    if(!context.Parameters.TryGetValue(variable.Type, out parameters))
                        context.Parameters.Add(variable.Type, parameters = new Dictionary<ParameterExpression, int>());
                    parameters.Add(variable, parameters.Count);
                }
            }
            CalcHashCode(node.Expressions, context);
            if(!context.Strictly)
            {
                foreach(var variable in node.Variables)
                    context.Parameters[variable.Type].Remove(variable);
            }
        }

        private static void CalcHashCodeCall(MethodCallExpression node, Context context)
        {
            context.HashCodes.Add(CalcHashCode(node.Method));
            CalcHashCode(node.Object, context);
            CalcHashCode(node.Arguments, context);
        }

        private static void CalcHashCodeConditional(ConditionalExpression node, Context context)
        {
            CalcHashCode(node.Test, context);
            CalcHashCode(node.IfTrue, context);
            CalcHashCode(node.IfFalse, context);
        }

        private static void CalcHashCodeConstant(ConstantExpression node, Context context)
        {
            context.HashCodes.Add(CalcHashCodeObject(node.Value));
        }

        private static void CalcHashCodeDebugInfo(DebugInfoExpression node, Context context)
        {
            throw new NotSupportedException();
        }

        private static void CalcHashCodeDefault(DefaultExpression node, Context context)
        {
            //empty body
        }

        private static void CalcHashCodeDynamic(DynamicExpression node, Context context)
        {
            throw new NotSupportedException();
        }

        private static void CalcHashCodeExtension(Expression node, Context context)
        {
            throw new NotSupportedException();
        }

        private static void CalcHashCodeLabel(LabelTarget target, Context context)
        {
            if(target == null)
            {
                context.HashCodes.Add(0);
                return;
            }

            int labelId;
            if (!context.Labels.TryGetValue(target, out labelId))
                context.Labels.Add(target, labelId = context.Labels.Count);
            context.HashCodes.Add(CalcHashCode(labelId));
        }

        private static void CalcHashCodeGoto(GotoExpression node, Context context)
        {
            context.HashCodes.Add(CalcHashCode(node.Kind));
            CalcHashCodeLabel(node.Target, context);
            CalcHashCode(node.Value, context);
        }

        private static void CalcHashCodeIndex(IndexExpression node, Context context)
        {
            CalcHashCode(node.Object, context);
            context.HashCodes.Add(CalcHashCode(node.Indexer));
            CalcHashCode(node.Arguments, context);
        }

        private static void CalcHashCodeInvoke(InvocationExpression node, Context context)
        {
            CalcHashCode(new[] {node.Expression}.Concat(node.Arguments), context);
        }

        private static void CalcHashCodeLabel(LabelExpression node, Context context)
        {
            int labelId;
            if (!context.Labels.TryGetValue(node.Target, out labelId))
            {
                labelId = context.Labels.Count;
                context.HashCodes.Add(labelId);
            }
            context.HashCodes.Add(labelId);
            CalcHashCode(node.DefaultValue, context);
        }

        private static void CalcHashCodeLambda(LambdaExpression node, Context context)
        {
            if(!context.Strictly)
            {
                foreach(var parameter in node.Parameters)
                {
                    Dictionary<ParameterExpression, int> parameters;
                    if(!context.Parameters.TryGetValue(parameter.Type, out parameters))
                        context.Parameters.Add(parameter.Type, parameters = new Dictionary<ParameterExpression, int>());
                    parameters.Add(parameter, parameters.Count);
                }
            }
            CalcHashCode(node.Body, context);
            if(!context.Strictly)
            {
                foreach(var parameter in node.Parameters)
                    context.Parameters[parameter.Type].Remove(parameter);
            }
        }

        private static void CalcHashCodeElemInits(IEnumerable<ElementInit> inits, Context context)
        {
            foreach(var init in inits)
            {
                context.HashCodes.Add(CalcHashCode(init.AddMethod));
                CalcHashCode(init.Arguments, context);
            }
        }

        private static void CalcHashCodeListInit(ListInitExpression node, Context context)
        {
            CalcHashCode(node.NewExpression, context);
            CalcHashCodeElemInits(node.Initializers, context);
        }

        private static void CalcHashCodeLoop(LoopExpression node, Context context)
        {
            CalcHashCode(node.Body, context);
            CalcHashCodeLabel(node.ContinueLabel, context);
            CalcHashCodeLabel(node.BreakLabel, context);
        }

        private static void CalcHashCodeMemberAccess(MemberExpression node, Context context)
        {
            context.HashCodes.Add(CalcHashCode(node.Member));
            CalcHashCode(node.Expression, context);
        }

        private static void CalcHashCodeMemberInit(MemberInitExpression node, Context context)
        {
            CalcHashCode(node.NewExpression, context);
            foreach(var memberBinding in node.Bindings)
            {
                var binding = (MemberAssignment)memberBinding;
                context.HashCodes.Add(CalcHashCode(binding.BindingType));
                context.HashCodes.Add(CalcHashCode(binding.Member));
                CalcHashCode(binding.Expression, context);
            }
        }

        private static void CalcHashCodeNew(NewExpression node, Context context)
        {
            context.HashCodes.Add(CalcHashCode(node.Constructor));
            CalcHashCode(node.Arguments, context);

            if(node.Members != null)
            {
                var normalizedMembers = node.Members.ToList();
                if(!context.Strictly)
                {
                    normalizedMembers.Sort((first, second) =>
                    {
                        if(first.Module != second.Module)
                            return string.Compare(first.Module.FullyQualifiedName, second.Module.FullyQualifiedName, StringComparison.InvariantCulture);
                        return first.MetadataToken - second.MetadataToken;
                    });
                }
                context.HashCodes.AddRange(node.Members.Select(CalcHashCode));
            }
        }

        private static void CalcHashCodeNewArray(NewArrayExpression node, Context context)
        {
            CalcHashCode(node.Expressions, context);
        }

        private static void CalcHashCodeRuntimeVariables(RuntimeVariablesExpression node, Context context)
        {
            throw new NotSupportedException();
        }

        private static void CalcHashCodeCases(IEnumerable<SwitchCase> cases, Context context)
        {
            foreach(var oneCase in cases)
            {
                CalcHashCode(oneCase.Body, context);
                CalcHashCode(oneCase.TestValues, context);
            }
        }

        private static void CalcHashCodeSwitch(SwitchExpression node, Context context)
        {
            CalcHashCodeCases(node.Cases, context);
            context.HashCodes.Add(CalcHashCode(node.Comparison));
            CalcHashCode(node.DefaultBody, context);
            CalcHashCode(node.SwitchValue, context);
        }

        private static void CalcHashCodeCatchBlocks(IEnumerable<CatchBlock> handlers, Context context)
        {
            foreach(var handler in handlers)
            {
                CalcHashCode(handler.Body, context);
                CalcHashCode(handler.Filter, context);
                context.HashCodes.Add(CalcHashCode(handler.Test));
                CalcHashCodeParameter(handler.Variable, context);
            }
        }

        private static void CalcHashCodeTry(TryExpression node, Context context)
        {
            CalcHashCode(node.Body, context);
            CalcHashCode(node.Fault, context);
            CalcHashCode(node.Finally, context);
            CalcHashCodeCatchBlocks(node.Handlers, context);
        }

        private static void CalcHashCodeTypeBinary(TypeBinaryExpression node, Context context)
        {
            CalcHashCode(node.Expression, context);
            context.HashCodes.Add(CalcHashCode(node.TypeOperand));
        }

        private class Context
        {
            public bool Strictly { get; set; }
            public Dictionary<Type, Dictionary<ParameterExpression, int>> Parameters { get; set; }
            public Dictionary<LabelTarget, int> Labels { get; set; }
            public List<int> HashCodes { get; set; }
        }
    }
}