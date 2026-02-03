using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.ArithmeticTests
{
    public class TestIsTrueFalse : TestBase
    {
        [Test]
        public void TestIsTrue1()
        {
            Expression<Func<bool>> exp = Expression.Lambda<Func<bool>>(Expression.IsTrue(Expression.Constant(true)));
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsTrue(f());
        }

        [Test]
        public void TestIsTrue2()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int));
            Expression<Func<int, bool>> exp = Expression.Lambda<Func<int, bool>>(Expression.IsTrue(Expression.NotEqual(parameter, Expression.Constant(0))), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsTrue(f(10));
            ClassicAssert.IsFalse(f(0));
        }

        [Test]
        public void TestIsTrue3()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TestClassA));
            Expression<Func<TestClassA, bool>> exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.IsTrue(Expression.GreaterThanOrEqual(Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetProperty("X")), Expression.Constant(0, typeof(int?)))), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsFalse(f(null));
            ClassicAssert.IsFalse(f(new TestClassA()));
            ClassicAssert.IsFalse(f(new TestClassA {X = -1}));
            ClassicAssert.IsTrue(f(new TestClassA {X = 0}));
        }

        [Test]
        public void TestIsFalse1()
        {
            Expression<Func<bool>> exp = Expression.Lambda<Func<bool>>(Expression.IsFalse(Expression.Constant(false)));
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsTrue(f());
        }

        [Test]
        public void TestIsFalse2()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int));
            Expression<Func<int, bool>> exp = Expression.Lambda<Func<int, bool>>(Expression.IsFalse(Expression.NotEqual(parameter, Expression.Constant(0))), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsFalse(f(10));
            ClassicAssert.IsTrue(f(0));
        }

        [Test]
        public void TestIsFalse3()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TestClassA));
            Expression<Func<TestClassA, bool>> exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.IsFalse(Expression.GreaterThanOrEqual(Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetProperty("X")), Expression.Constant(0, typeof(int?)))), parameter);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsFalse(f(null));
            ClassicAssert.IsFalse(f(new TestClassA()));
            ClassicAssert.IsTrue(f(new TestClassA {X = -1}));
            ClassicAssert.IsFalse(f(new TestClassA {X = 0}));
        }

        public class TestClassA
        {
            public int? X { get; set; }
        }
    }
}