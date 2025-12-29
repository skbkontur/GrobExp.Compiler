using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.PowerAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double), "a");
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<double, double, double>> exp = Expression.Lambda<Func<double, double, double>>(Expression.PowerAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0, 0));
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(16, f(2, 4));
            ClassicAssert.AreEqual(1, f(-1, 2));

            exp = Expression.Lambda<Func<double, double, double>>(Expression.Block(typeof(double), Expression.PowerAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0, 0));
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(16, f(2, 4));
            ClassicAssert.AreEqual(1, f(-1, 2));
        }

        [Test]
        public void TestNullableDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double?), "a");
            ParameterExpression b = Expression.Parameter(typeof(double?), "b");
            Expression<Func<double?, double?, double?>> exp = Expression.Lambda<Func<double?, double?, double?>>(Expression.PowerAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0, 0));
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(16, f(2, 4));
            ClassicAssert.AreEqual(1, f(-1, 2));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<double?, double?, double?>>(Expression.Block(typeof(double?), Expression.PowerAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
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