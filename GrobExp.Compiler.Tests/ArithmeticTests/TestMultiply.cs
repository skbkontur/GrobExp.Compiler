using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.ArithmeticTests
{
    public class TestMultiply : TestBase
    {
        [Test]
        public void Test1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a * b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 1));
            ClassicAssert.AreEqual(2, f(1, 2));
            ClassicAssert.AreEqual(6, f(-2, -3));
            ClassicAssert.AreEqual(-20, f(-2, 10));
            unchecked
            {
                ClassicAssert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }
        }

        [Test]
        public void Test2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a * b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(2, f(1, 2));
            ClassicAssert.AreEqual(6, f(-2, -3));
            ClassicAssert.AreEqual(-20, f(-2, 10));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
            unchecked
            {
                ClassicAssert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }
        }

        [Test]
        public void Test3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a * b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(2, f(1, 2));
            ClassicAssert.AreEqual(6, f(-2, -3));
            ClassicAssert.AreEqual(-20, f(-2, 10));
            ClassicAssert.AreEqual(-20000000000, f(2000000000, -10));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(int));
            ParameterExpression b = Expression.Parameter(typeof(int));
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.MultiplyChecked(a, b), a, b);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 1));
            ClassicAssert.AreEqual(2, f(1, 2));
            ClassicAssert.AreEqual(6, f(-2, -3));
            ClassicAssert.AreEqual(-20, f(-2, 10));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?));
            ParameterExpression b = Expression.Parameter(typeof(int?));
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.MultiplyChecked(a, b), a, b);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(2, f(1, 2));
            ClassicAssert.AreEqual(6, f(-2, -3));
            ClassicAssert.AreEqual(-20, f(-2, 10));
            ClassicAssert.AreEqual(-2000000000, f(200000000, -10));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint));
            ParameterExpression b = Expression.Parameter(typeof(uint));
            Expression<Func<uint, uint, uint>> exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.MultiplyChecked(a, b), a, b);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 1));
            ClassicAssert.AreEqual(2, f(1, 2));
            ClassicAssert.AreEqual(4000000000, f(2000000000, 2));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void Test7()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?));
            ParameterExpression b = Expression.Parameter(typeof(uint?));
            Expression<Func<uint?, uint?, uint?>> exp = Expression.Lambda<Func<uint?, uint?, uint?>>(Expression.MultiplyChecked(a, b), a, b);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(2, f(1, 2));
            ClassicAssert.AreEqual(4000000000, f(2000000000, 2));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void Test8()
        {
            Expression<Func<int?, int, int?>> exp = (a, b) => a * b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(2, f(1, 2));
            ClassicAssert.AreEqual(6, f(-2, -3));
            ClassicAssert.AreEqual(-20, f(-2, 10));
            ClassicAssert.IsNull(f(null, 2));
            unchecked
            {
                ClassicAssert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }
        }
    }
}