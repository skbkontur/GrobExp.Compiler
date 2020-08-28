using System;
using System.Linq.Expressions;

using NUnit.Framework;

namespace GrobExp.Compiler.Tests
{
    [TestFixture]
    public class TestParameterExtractor
    {
        [Test]
        public void ParameterExpression()
        {
            var parameter = Expression.Parameter(typeof(int));

            Assert.That(extractor.Extract(parameter), Is.EqualTo(new object[] {parameter}));
        }

        [Test]
        public void TwoUsagesOfParameter()
        {
            var parameter = Expression.Parameter(typeof(int));
            var sum = Expression.Add(parameter, parameter);

            Assert.That(extractor.Extract(sum), Is.EqualTo(new[] {parameter}));
        }

        [Test]
        public void TwoParametersOfDifferentTypes()
        {
            var intParameter = Expression.Parameter(typeof(int));
            var doubleParameter = Expression.Parameter(typeof(double));
            var sum = Expression.Add(doubleParameter, Expression.Convert(intParameter, typeof(double)));

            Assert.That(extractor.Extract(sum), Is.EqualTo(new[] {doubleParameter, intParameter}));
        }

        [Test]
        public void TwoParametersOfSameType()
        {
            var firstParameter = Expression.Parameter(typeof(int), "a");
            var secondParameter = Expression.Parameter(typeof(int), "a");

            var sum = Expression.Add(firstParameter, secondParameter);
            Assert.That(extractor.Extract(sum), Is.EqualTo(new[] {firstParameter, secondParameter}));
        }

        [Test]
        public void TwoVariablesOfSameType()
        {
            var firstParameter = Expression.Variable(typeof(int), "a");
            var secondParameter = Expression.Variable(typeof(int), "a");

            var sum = Expression.Add(firstParameter, secondParameter);
            Assert.That(extractor.Extract(sum), Is.EqualTo(new[] {firstParameter, secondParameter}));
        }

        [Test]
        public void IgnoreLocalVariables()
        {
            var firstParameter = Expression.Parameter(typeof(int));
            var secondParameter = Expression.Parameter(typeof(int));

            var variable = Expression.Variable(typeof(int));
            var assign = Expression.Assign(variable, Expression.Constant(5));

            var sum = Expression.Add(Expression.Add(variable, firstParameter), secondParameter);
            var block = Expression.Block(typeof(int), variables : new[] {variable}, expressions : new Expression[] {assign, sum});

            Assert.That(extractor.Extract(block), Is.EqualTo(new[] {firstParameter, secondParameter}));
        }

        [Test]
        public void IgnoreNestedLambdaParameters()
        {
            var firstParameter = Expression.Parameter(typeof(int));
            var secondParameter = Expression.Parameter(typeof(int));

            var lambdaParameter = Expression.Variable(typeof(int));
            var body = Expression.Multiply(Expression.Add(lambdaParameter, firstParameter), secondParameter);

            var lambda = Expression.Lambda(body, lambdaParameter);
            Assert.That(extractor.Extract(lambda), Is.EqualTo(new[] {firstParameter, secondParameter}));
        }

        [Test]
        public void ExtractFromPureLambda()
        {
            Expression<Func<A, B, string, string>> lambda = (a, b, s) => s + a.B.S + b.S + a.S;
            Assert.That(extractor.Extract(lambda.Body), Is.EqualTo(new[] {lambda.Parameters[2], lambda.Parameters[0], lambda.Parameters[1]}));
        }

        private readonly ParametersExtractor extractor = new ParametersExtractor();

        private class A
        {
            public string S { get; set; }

            public B B { get; set; }
        }

        public class B
        {
            public string S { get; set; }
        }
    }
}