using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.ArithmeticTests
{
    public class TestPower : TestBase
    {
        [Test]
        public void TestPower1()
        {
            ParameterExpression a = Expression.Parameter(typeof(double));
            ParameterExpression b = Expression.Parameter(typeof(double));
            var exp = Expression.Lambda<Func<double, double, double>>(Expression.Power(a, b), a, b);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0, 0));
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(16, f(2, 4));
            Assert.AreEqual(1, f(-1, 2));
        }

        [Test]
        public void TestPower2()
        {
            ParameterExpression a = Expression.Parameter(typeof(double?));
            ParameterExpression b = Expression.Parameter(typeof(double?));
            var exp = Expression.Lambda<Func<double?, double?, double?>>(Expression.Power(a, b), a, b);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0, 0));
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(16, f(2, 4));
            Assert.AreEqual(1, f(-1, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }
    }
}