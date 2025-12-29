using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests
{
    [TestFixture]
    public class TestUnbox
    {
        [Test]
        public void Test1()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(object));
            Expression<Func<object, int>> exp = Expression.Lambda<Func<object, int>>(Expression.Unbox(parameter, typeof(int)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(-1));
            ClassicAssert.AreEqual(1, f(1));
            ClassicAssert.AreEqual(0, f(null));
            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            ClassicAssert.AreEqual(-1, f(-1));
            ClassicAssert.AreEqual(1, f(1));
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TestClassA));
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(Expression.Unbox(Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("X")), typeof(int)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(new TestClassA {X = -1}));
            ClassicAssert.AreEqual(1, f(new TestClassA {X = 1}));
            ClassicAssert.AreEqual(0, f(null));
            ClassicAssert.AreEqual(0, f(new TestClassA()));
            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            ClassicAssert.AreEqual(-1, f(new TestClassA {X = -1}));
            ClassicAssert.AreEqual(1, f(new TestClassA {X = 1}));
            Assert.Throws<NullReferenceException>(() => f(new TestClassA()));
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        [Test]
        public void Test3()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(object));
            Expression<Func<object, TestEnum>> exp = Expression.Lambda<Func<object, TestEnum>>(Expression.Unbox(parameter, typeof(TestEnum)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(TestEnum.One, f(TestEnum.One));
            ClassicAssert.AreEqual(TestEnum.Two, f(TestEnum.Two));
            ClassicAssert.AreEqual(TestEnum.Zero, f(null));
            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            ClassicAssert.AreEqual(TestEnum.One, f(TestEnum.One));
            ClassicAssert.AreEqual(TestEnum.Two, f(TestEnum.Two));
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        public enum TestEnum
        {
            Zero,
            One,
            Two
        }

        public class TestClassA
        {
            public object X;
        }
    }
}