using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestNotEqual
    {
        [Test]
        public void Test1()
        {
            Expression<Func<int, int, bool>> exp = (a, b) => a != b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(true, f(0, 1));
            Assert.AreEqual(false, f(1, 1));
        }

        [Test]
        public void Test2()
        {
            Expression<Func<int, long, bool>> exp = (a, b) => a != b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(true, f(0, 1));
            Assert.AreEqual(false, f(1, 1));
        }

        [Test]
        public void Test3()
        {
            Expression<Func<long, long, bool>> exp = (a, b) => a != b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(true, f(0, 1));
            Assert.AreEqual(false, f(1, 1));
        }

        [Test]
        public void Test4()
        {
            Expression<Func<string, string, bool>> exp = (a, b) => a != b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(true, f("zzz", "qxx"));
            Assert.AreEqual(false, f("zzz", "zzz"));
        }

        [Test]
        public void Test5()
        {
            Expression<Func<int, decimal, bool>> exp = (a, b) => a != b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(true, f(0, 1m));
            Assert.AreEqual(false, f(1, 1m));
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?));
            Expression body = Expression.NotEqual(a, Expression.Constant(null));
            Expression<Func<int?, bool>> exp = Expression.Lambda<Func<int?, bool>>(body, a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(true, f(1));
            Assert.AreEqual(false, f(null));
        }
    }
}