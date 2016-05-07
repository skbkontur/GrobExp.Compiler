using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace GrobExp.Compiler
{
    internal class ExpressionClosureResolver : ExpressionVisitor
    {
        public ExpressionClosureResolver(LambdaExpression lambda, ModuleBuilder module, bool dynamic)
        {
            lambda = (LambdaExpression)new LambdaPreparer().Visit(new RuntimeVariablesInliner().Visit(lambda));
            if(!dynamic)
                lambda = (LambdaExpression)new ExpressionPrivateMembersAccessor().Visit(new ExpressionAnonymousTypeReplacer(module).Visit(lambda));
            var parsedLambda = new ExpressionClosureBuilder(lambda, module).Build(dynamic);
            this.lambda = parsedLambda.Lambda;
            closureType = parsedLambda.ClosureType;
            closureParameter = parsedLambda.ClosureParameter;
            constantsType = parsedLambda.ConstantsType;
            constantsParameter = parsedLambda.ConstantsParameter;
            constants = parsedLambda.Constants;
            parsedParameters = parsedLambda.ParsedParameters;
            parsedConstants = parsedLambda.ParsedConstants;
            parsedSwitches = parsedLambda.ParsedSwitches;
        }

        public LambdaExpression Resolve(out Type closureType, out ParameterExpression closureParameter, out Type constantsType, out ParameterExpression constantsParameter, out object constants, out Dictionary<SwitchExpression, Tuple<FieldInfo, FieldInfo, int>> switches)
        {
            var body = ((LambdaExpression)Visit(lambda)).Body;
            closureParameter = this.closureParameter;
            closureType = this.closureType;
            constantsParameter = this.constantsParameter;
            constantsType = this.constantsType;
            constants = this.constants;
            switches = parsedSwitches;
            if(closureParameter != null)
                body = Expression.Block(new[] {closureParameter}, Expression.Assign(closureParameter, Expression.New(closureType)), body);
            var parameters = (constantsParameter == null ? lambda.Parameters : new[] {constantsParameter}.Concat(lambda.Parameters)).ToArray();
            var delegateType = Extensions.GetDelegateType(parameters.Select(parameter => parameter.Type).ToArray(), lambda.ReturnType);
            return Expression.Lambda(delegateType, body, lambda.Name, lambda.TailCall, parameters);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            localParameters.Push(new HashSet<ParameterExpression>(node.Parameters));
            var body = base.Visit(node.Body);
            localParameters.Pop();
            var assigns = new List<Expression>();
            foreach(var parameter in node.Parameters)
            {
                FieldInfo field;
                if(parsedParameters.TryGetValue(parameter, out field))
                    assigns.Add(Expression.Assign(Expression.MakeMemberAccess(closureParameter, field), parameter.Type == field.FieldType ? (Expression)parameter : Expression.New(field.FieldType.GetConstructor(new[] {parameter.Type}), parameter)));
            }
            return Expression.Lambda<T>(assigns.Count == 0 ? body : Expression.Block(body.Type, assigns.Concat(new[] {body})), node.Name, node.TailCall, node.Parameters);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var peek = localParameters.Peek();
            var variables = node.Variables.Where(variable => !parsedParameters.ContainsKey(variable) && !peek.Contains(variable)).ToArray();
            foreach(var variable in variables)
                peek.Add(variable);
            var expressions = node.Expressions.Select(Visit);
            foreach(var variable in variables)
                peek.Remove(variable);
            return node.Update(variables, expressions);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            var peek = localParameters.Peek();
            var variable = node.Variable;
            if(variable != null && (peek.Contains(variable) || parsedParameters.ContainsKey(variable)))
                variable = null;
            if(variable != null)
                peek.Add(variable);
            var res = base.VisitCatchBlock(node);
            if(variable != null)
                peek.Remove(variable);
            return res;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            FieldInfo field;
            Expression result = parsedConstants.TryGetValue(node, out field)
                                    ? (field.FieldType == node.Type
                                           ? Expression.MakeMemberAccess(constantsParameter, field)
                                           : Expression.MakeMemberAccess(Expression.MakeMemberAccess(constantsParameter, field), field.FieldType.GetField("Value", BindingFlags.Public | BindingFlags.Instance)))
                                    : base.VisitConstant(node);
            if(node.Value is Expression)
            {
                if(closureParameter != null)
                {
                    var exp = (Expression)node.Value;
                    var temp = new ClosureSubstituter(closureParameter, parsedParameters).Visit(exp);
                    if(temp != exp)
                    {
                        var constructor = typeof(ExpressionQuoter).GetConstructor(new[] {typeof(object)});
                        result = Expression.Convert(Expression.Call(Expression.New(constructor, closureParameter), typeof(ExpressionVisitor).GetMethod("Visit", new[] {typeof(Expression)}), new[] {result}), node.Type);
                    }
                }
            }
            return result;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            FieldInfo field;
            return (!localParameters.Peek().Contains(node) || node.Type.IsValueType) && parsedParameters.TryGetValue(node, out field)
                       ? node.Type == field.FieldType
                             ? Expression.MakeMemberAccess(closureParameter, field)
                             : Expression.MakeMemberAccess(Expression.MakeMemberAccess(closureParameter, field), field.FieldType.GetField("Value", BindingFlags.Public | BindingFlags.Instance))
                       : base.VisitParameter(node);
        }

        private readonly Stack<HashSet<ParameterExpression>> localParameters = new Stack<HashSet<ParameterExpression>>();

        private readonly LambdaExpression lambda;
        private readonly ParameterExpression closureParameter;
        private readonly Type closureType;
        private readonly ParameterExpression constantsParameter;
        private readonly Type constantsType;
        private readonly object constants;
        private readonly Dictionary<ConstantExpression, FieldInfo> parsedConstants;
        private readonly Dictionary<ParameterExpression, FieldInfo> parsedParameters;
        private readonly Dictionary<SwitchExpression, Tuple<FieldInfo, FieldInfo, int>> parsedSwitches;
    }
}