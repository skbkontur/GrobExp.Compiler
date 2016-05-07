using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    public class TestRuntimeVariables : TestBase
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(int));
            ParameterExpression b = Expression.Parameter(typeof(int));
            Expression<Func<int, int, IRuntimeVariables>> exp =
                Expression.Lambda<Func<int, int, IRuntimeVariables>>(
                    Expression.Block(
                        Expression.AddAssign(a, b),
                        Expression.RuntimeVariables(a, b)
                        ), a, b);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(3, f(1, 2)[0]);
            Assert.AreEqual(2, f(1, 2)[1]);
        }
    }
}