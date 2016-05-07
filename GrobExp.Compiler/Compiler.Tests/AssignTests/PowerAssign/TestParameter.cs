using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PowerAssign
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
            Assert.AreEqual(1, f(0, 0));
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(16, f(2, 4));
            Assert.AreEqual(1, f(-1, 2));

            exp = Expression.Lambda<Func<double, double, double>>(Expression.Block(typeof(double), Expression.PowerAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0, 0));
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(16, f(2, 4));
            Assert.AreEqual(1, f(-1, 2));
        }

        [Test]
        public void TestNullableDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double?), "a");
            ParameterExpression b = Expression.Parameter(typeof(double?), "b");
            Expression<Func<double?, double?, double?>> exp = Expression.Lambda<Func<double?, double?, double?>>(Expression.PowerAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0, 0));
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(16, f(2, 4));
            Assert.AreEqual(1, f(-1, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<double?, double?, double?>>(Expression.Block(typeof(double?), Expression.PowerAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
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