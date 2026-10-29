using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.ArithmeticTests
{
    public class TestIncrement : TestBase
    {
        [Test]
        public void Test1()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int));
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(2, f(1));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(int.MinValue, f(int.MaxValue));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int?));
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(2, f(1));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(int.MinValue, f(int.MaxValue));
            ClassicAssert.IsNull(f(null));
        }

        [Test]
        public void Test3()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(long));
            Expression<Func<long, long>> exp = Expression.Lambda<Func<long, long>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(2, f(1));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(long.MinValue, f(long.MaxValue));
        }

        [Test]
        public void Test4()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(long?));
            Expression<Func<long?, long?>> exp = Expression.Lambda<Func<long?, long?>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(2, f(1));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(long.MinValue, f(long.MaxValue));
            ClassicAssert.IsNull(f(null));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(double));
            Expression<Func<double, double>> exp = Expression.Lambda<Func<double, double>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1.5, f(0.5));
            ClassicAssert.AreEqual(2.125, f(1.125));
            ClassicAssert.AreEqual(0, f(-1));
        }

        [Test]
        public void Test6()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(double?));
            Expression<Func<double?, double?>> exp = Expression.Lambda<Func<double?, double?>>(Expression.Increment(parameter), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1.5, f(0.5));
            ClassicAssert.AreEqual(2.125, f(1.125));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.IsNull(f(null));
        }
    }
}