using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.DivideAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.DivideAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));

            exp = Expression.Lambda<Func<int, int, int>>(Expression.Block(typeof(int), Expression.DivideAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.DivideAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.Block(typeof(int?), Expression.DivideAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double), "a");
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<double, double, double>> exp = Expression.Lambda<Func<double, double, double>>(Expression.DivideAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0.5, f(1, 2));
            Assert.AreEqual(2.5, f(5, 2));
            Assert.AreEqual(-1.5, f(-3, 2));

            exp = Expression.Lambda<Func<double, double, double>>(Expression.Block(typeof(double), Expression.DivideAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0.5, f(1, 2));
            Assert.AreEqual(2.5, f(5, 2));
            Assert.AreEqual(-1.5, f(-3, 2));
        }

        [Test]
        public void TestUInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<uint, uint, uint>> exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.DivideAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(2147483646, f(uint.MaxValue - 3 + 1, 2));

            exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.Block(typeof(uint), Expression.DivideAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(2147483646, f(uint.MaxValue - 3 + 1, 2));
        }
    }
}