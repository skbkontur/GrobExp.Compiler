using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.ArithmeticTests
{
    public class TestIncrement : TestBase
    {
        [Test]
        public void Test1()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int));
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(2, f(1));
            Assert.AreEqual(0, f(-1));
            Assert.AreEqual(int.MinValue, f(int.MaxValue));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int?));
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(2, f(1));
            Assert.AreEqual(0, f(-1));
            Assert.AreEqual(int.MinValue, f(int.MaxValue));
            Assert.IsNull(f(null));
        }

        [Test]
        public void Test3()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(long));
            Expression<Func<long, long>> exp = Expression.Lambda<Func<long, long>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(2, f(1));
            Assert.AreEqual(0, f(-1));
            Assert.AreEqual(long.MinValue, f(long.MaxValue));
        }

        [Test]
        public void Test4()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(long?));
            Expression<Func<long?, long?>> exp = Expression.Lambda<Func<long?, long?>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(2, f(1));
            Assert.AreEqual(0, f(-1));
            Assert.AreEqual(long.MinValue, f(long.MaxValue));
            Assert.IsNull(f(null));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(double));
            Expression<Func<double, double>> exp = Expression.Lambda<Func<double, double>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1.5, f(0.5));
            Assert.AreEqual(2.125, f(1.125));
            Assert.AreEqual(0, f(-1));
        }

        [Test]
        public void Test6()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(double?));
            Expression<Func<double?, double?>> exp = Expression.Lambda<Func<double?, double?>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1.5, f(0.5));
            Assert.AreEqual(2.125, f(1.125));
            Assert.AreEqual(0, f(-1));
            Assert.IsNull(f(null));
        }
    }
}