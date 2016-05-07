using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestEqual
    {
        [Test]
        public void Test1()
        {
            Expression<Func<int, int, bool>> exp = (a, b) => a == b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f(0, 1));
            Assert.AreEqual(true, f(1, 1));
        }

        [Test]
        public void Test2()
        {
            Expression<Func<int, long, bool>> exp = (a, b) => a == b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f(0, 1));
            Assert.AreEqual(true, f(1, 1));
        }

        [Test]
        public void Test3()
        {
            Expression<Func<long, long, bool>> exp = (a, b) => a == b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f(0, 1));
            Assert.AreEqual(true, f(1, 1));
        }

        [Test]
        public void Test4()
        {
            Expression<Func<string, string, bool>> exp = (a, b) => a == b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f("zzz", "qxx"));
            Assert.AreEqual(true, f("zzz", "zzz"));
        }

        [Test]
        public void Test5()
        {
            Expression<Func<int, decimal, bool>> exp = (a, b) => a == b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f(0, 1m));
            Assert.AreEqual(true, f(1, 1m));
        }

        [Test]
        public void Test6()
        {
            Expression<Func<TestEnum, TestEnum, bool>> exp = (a, b) => a == b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f(TestEnum.Zero, TestEnum.One));
            Assert.AreEqual(true, f(TestEnum.One, TestEnum.One));
        }

        [Test]
        public void Test7()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestEnum));
            ParameterExpression b = Expression.Parameter(typeof(TestEnum));
            Expression body = Expression.Equal(a, b);
            Expression<Func<TestEnum, TestEnum, bool>> exp = Expression.Lambda<Func<TestEnum, TestEnum, bool>>(body, a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f(TestEnum.Zero, TestEnum.One));
            Assert.AreEqual(true, f(TestEnum.One, TestEnum.One));
        }

        [Test]
        public void Test8()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?));
            Expression body = Expression.Equal(a, Expression.Constant(null));
            Expression<Func<int?, bool>> exp = Expression.Lambda<Func<int?, bool>>(body, a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f(1));
            Assert.AreEqual(true, f(null));
        }

        private enum TestEnum
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10,
            Eleven = 11,
            Twelve = 12
        }
    }
}