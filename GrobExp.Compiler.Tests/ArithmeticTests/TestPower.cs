using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.ArithmeticTests
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
            ClassicAssert.AreEqual(1, f(0, 0));
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(16, f(2, 4));
            ClassicAssert.AreEqual(1, f(-1, 2));
        }

        [Test]
        public void TestPower2()
        {
            ParameterExpression a = Expression.Parameter(typeof(double?));
            ParameterExpression b = Expression.Parameter(typeof(double?));
            var exp = Expression.Lambda<Func<double?, double?, double?>>(Expression.Power(a, b), a, b);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0, 0));
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(16, f(2, 4));
            ClassicAssert.AreEqual(1, f(-1, 2));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
        }
    }
}