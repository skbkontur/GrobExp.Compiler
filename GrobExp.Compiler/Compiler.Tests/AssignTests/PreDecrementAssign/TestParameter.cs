using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PreDecrementAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.PreDecrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-1, f(0));
            Assert.AreEqual(0, f(1));
            Assert.AreEqual(int.MaxValue, f(int.MinValue));

            exp = Expression.Lambda<Func<int, int>>(Expression.Block(typeof(int), Expression.PreDecrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-1, f(0));
            Assert.AreEqual(0, f(1));
            Assert.AreEqual(int.MaxValue, f(int.MinValue));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.PreDecrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-1, f(0));
            Assert.AreEqual(0, f(1));
            Assert.AreEqual(int.MaxValue, f(int.MinValue));
            Assert.IsNull(f(null));

            exp = Expression.Lambda<Func<int?, int?>>(Expression.Block(typeof(int?), Expression.PreDecrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-1, f(0));
            Assert.AreEqual(0, f(1));
            Assert.AreEqual(int.MaxValue, f(int.MinValue));
            Assert.IsNull(f(null));
        }

        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double), "a");
            Expression<Func<double, double>> exp = Expression.Lambda<Func<double, double>>(Expression.PreDecrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-1, f(0));
            Assert.AreEqual(0, f(1));
            Assert.AreEqual(-0.5, f(0.5));

            exp = Expression.Lambda<Func<double, double>>(Expression.Block(typeof(double), Expression.PreDecrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-1, f(0));
            Assert.AreEqual(0, f(1));
            Assert.AreEqual(-0.5, f(0.5));
        }
    }
}