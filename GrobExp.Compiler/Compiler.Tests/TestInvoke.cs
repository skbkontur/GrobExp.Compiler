using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestInvoke
    {
        [Test]
        public void Test1()
        {
            Func<int, int, int> func = (a, b) => a + b;
            Expression<Func<int, int, int>> exp = (a, b) => func(a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(3, f(1, 2));
        }

        [Test]
        public void Test2()
        {
            Expression<Func<int, int, int>> lambda = (a, b) => a + b;
            ParameterExpression parameterA = Expression.Parameter(typeof(int));
            ParameterExpression parameterB = Expression.Parameter(typeof(int));
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.Invoke(lambda, parameterA, parameterB), parameterA, parameterB);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(3, f(1, 2));
        }

        [Test]
        public void TestWithLabels1()
        {
            var target = Expression.Label(typeof(int));
            Expression body = Expression.Block(Expression.Goto(target, Expression.Constant(1)), Expression.Label(target, Expression.Constant(2)));
            Expression<Func<int>> lambda = Expression.Lambda<Func<int>>(body);
            Expression<Func<int>> exp = Expression.Lambda<Func<int>>(Expression.Block(Expression.Invoke(lambda), Expression.Invoke(lambda)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f());
        }
    }
}