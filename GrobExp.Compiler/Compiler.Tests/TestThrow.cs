using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestThrow
    {
        [Test]
        public void Test1()
        {
            Expression<Action> exp = Expression.Lambda<Action>(Expression.Throw(Expression.Constant(new Exception())));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.Throws<Exception>(() => f());
        }

        [Test]
        public void Test2()
        {
            Expression<Action> exp = Expression.Lambda<Action>(Expression.Throw(Expression.New(typeof(Exception))));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.Throws<Exception>(() => f());
        }
    }
}