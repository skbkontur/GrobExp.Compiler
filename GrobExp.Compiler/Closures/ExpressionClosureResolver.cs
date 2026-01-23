using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace GrobExp.Compiler.Closures
{
    internal class ExpressionClosureResolver : ExpressionVisitor
    {
        public ExpressionClosureResolver(LambdaExpression lambda, ModuleBuilder module, bool dynamic, CompilerOptions options)
        {
            lambda = (LambdaExpression)new LambdaPreparer().Visit(new RuntimeVariablesInliner().Visit(lambda));
            if (!dynamic)
                lambda = (LambdaExpression)new ExpressionPrivateMembersAccessor().Visit(new ExpressionAnonymousTypeReplacer(module).Visit(lambda));
            constantsBuilder = options.HasFlag(CompilerOptions.CreateDynamicClosure)
                                   ? new DynamicClosureBuilder(module)
                                   : (IClosureBuilder)new StaticClosureBuilder();
            closureBuilder = options.HasFlag(CompilerOptions.CreateDynamicClosure)
                                 ? new DynamicClosureBuilder(module)
                                 : (IClosureBuilder)new StaticClosureBuilder();
            parsedLambda = new ExpressionClosureBuilder(lambda, closureBuilder, constantsBuilder).Build(dynamic);
        }

        public LambdaExpression Resolve(out ParsedLambda parsedLambda)
        {
            var body = ((LambdaExpression)Visit(this.parsedLambda.Lambda)).Body;
            if (this.parsedLambda.ClosureParameter != null)
                body = Expression.Block(new[] {this.parsedLambda.ClosureParameter}, closureBuilder.Init(this.parsedLambda.ClosureParameter), body);
            var parameters = (this.parsedLambda.ConstantsParameter == null ? this.parsedLambda.Lambda.Parameters : new[] {this.parsedLambda.ConstantsParameter}.Concat(this.parsedLambda.Lambda.Parameters)).ToArray();
            var delegateType = Extensions.GetDelegateType(parameters.Select(parameter => parameter.Type).ToArray(), this.parsedLambda.Lambda.ReturnType);
            parsedLambda = new ParsedLambda
                {
                    ClosureBuilder = closureBuilder,
                    ClosureType = this.parsedLambda.ClosureType,
                    ClosureParameter = this.parsedLambda.ClosureParameter,
                    ConstantsBuilder = constantsBuilder,
                    ConstantsType = this.parsedLambda.ConstantsType,
                    ConstantsParameter = this.parsedLambda.ConstantsParameter,
                    DelegatesFieldId = this.parsedLambda.DelegatesFieldId,
                    Constants = this.parsedLambda.Constants,
                    ParsedConstants = this.parsedLambda.ParsedConstants,
                    ParsedParameters = this.parsedLambda.ParsedParameters,
                    ParsedSwitches = this.parsedLambda.ParsedSwitches
                };
            return Expression.Lambda(delegateType, body, this.parsedLambda.Lambda.Name, this.parsedLambda.Lambda.TailCall, parameters);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            localParameters.Push(new HashSet<ParameterExpression>(node.Parameters));
            var body = base.Visit(node.Body);
            localParameters.Pop();
            var assigns = new List<Expression>();
            foreach (var parameter in node.Parameters)
            {
                int field;
                if (parsedLambda.ParsedParameters.TryGetValue(parameter, out field))
                    assigns.Add(closureBuilder.Assign(parsedLambda.ClosureParameter, field, parameter));
//                    assigns.Add(Expression.Assign(closureBuilder.MakeAccess(parsedLambda.ClosureParameter, field),
//                        parameter.Type == field.FieldType ? (Expression)parameter : Expression.New(field.FieldType.GetConstructor(new[] {parameter.Type}), parameter)));
            }
            return Expression.Lambda<T>(assigns.Count == 0 ? body : Expression.Block(body.Type, assigns.Concat(new[] {body})), node.Name, node.TailCall, node.Parameters);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var peek = localParameters.Peek();
            var variables = node.Variables.Where(variable => !parsedLambda.ParsedParameters.ContainsKey(variable) && !peek.Contains(variable)).ToArray();
            foreach (var variable in variables)
                peek.Add(variable);
            var expressions = node.Expressions.Select(Visit);
            foreach (var variable in variables)
                peek.Remove(variable);
            return node.Update(variables, expressions);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            var peek = localParameters.Peek();
            var variable = node.Variable;
            if (variable != null && (peek.Contains(variable) || parsedLambda.ParsedParameters.ContainsKey(variable)))
                variable = null;
            if (variable != null)
                peek.Add(variable);
            var res = base.VisitCatchBlock(node);
            if (variable != null)
                peek.Remove(variable);
            return res;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            int field;
            Expression result = parsedLambda.ParsedConstants.TryGetValue(node, out field)
                                    ? constantsBuilder.MakeAccess(parsedLambda.ConstantsParameter, field)
                                    : base.VisitConstant(node);
            if (node.Value is Expression)
            {
                if (parsedLambda.ClosureParameter != null)
                {
                    var exp = (Expression)node.Value;
                    var temp = new ClosureSubstituter(parsedLambda.ClosureParameter, closureBuilder, parsedLambda.ParsedParameters).Visit(exp);
                    if (temp != exp)
                    {
                        var constructor = typeof(ExpressionQuoter).GetConstructor(new[] {typeof(object)});
                        result = Expression.Convert(Expression.Call(Expression.New(constructor, parsedLambda.ClosureParameter), typeof(ExpressionVisitor).GetMethod("Visit", new[] {typeof(Expression)}), new[] {result}), node.Type);
                    }
                }
            }
            return result;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            int field;
            return (!localParameters.Peek().Contains(node) || node.Type.IsValueType) && parsedLambda.ParsedParameters.TryGetValue(node, out field)
                       ? closureBuilder.MakeAccess(parsedLambda.ClosureParameter, field)
                       : base.VisitParameter(node);
        }

        private readonly Stack<HashSet<ParameterExpression>> localParameters = new Stack<HashSet<ParameterExpression>>();

        private readonly IClosureBuilder constantsBuilder;
        private readonly IClosureBuilder closureBuilder;
        private readonly ExpressionClosureBuilder.Result parsedLambda;
    }
}