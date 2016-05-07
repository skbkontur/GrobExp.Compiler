using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestLabels
    {
        [Test]
        public void Test1()
        {
            var target = Expression.Label(typeof(int));
            Expression body = Expression.Block(Expression.Goto(target, Expression.Constant(1)), Expression.Label(target, Expression.Constant(2)));
            Expression<Func<int>> exp = Expression.Lambda<Func<int>>(body);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f());
        }
    }
}