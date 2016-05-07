using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestTypeEqual
    {
        [Test]
        public void Test1()
        {
            var parameter = Expression.Parameter(typeof(int));
            var exp = Expression.Lambda<Func<int, bool>>(Expression.TypeEqual(parameter, typeof(double)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(5));
        }

        [Test]
        public void Test2()
        {
            var parameter = Expression.Parameter(typeof(int));
            var exp = Expression.Lambda<Func<int, bool>>(Expression.TypeEqual(parameter, typeof(int)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(5));
        }

        [Test]
        public void Test3()
        {
            var parameter = Expression.Parameter(typeof(int));
            var exp = Expression.Lambda<Func<int, bool>>(Expression.TypeEqual(parameter, typeof(object)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(5));
        }

        [Test]
        public void Test4()
        {
            var parameter = Expression.Parameter(typeof(TestClassB));
            var exp = Expression.Lambda<Func<TestClassB, bool>>(Expression.TypeEqual(parameter, typeof(TestClassA)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(new TestClassB()));
        }

        [Test]
        public void Test5()
        {
            var parameter = Expression.Parameter(typeof(TestClassA));
            var exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.TypeEqual(parameter, typeof(TestClassB)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(new TestClassA()));
            Assert.IsTrue(f(new TestClassB()));
        }

        [Test]
        public void Test6()
        {
            var parameter = Expression.Parameter(typeof(object));
            var exp = Expression.Lambda<Func<object, bool>>(Expression.TypeEqual(parameter, typeof(int)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(5));
            Assert.IsFalse(f(5.5));
        }

        [Test]
        public void Test7()
        {
            var parameter = Expression.Parameter(typeof(object));
            var exp = Expression.Lambda<Func<object, bool>>(Expression.TypeEqual(parameter, typeof(TestEnum)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(TestEnum.One));
            Assert.IsFalse(f(5.5));
        }

        [Test]
        public void Test8()
        {
            var parameter = Expression.Parameter(typeof(TestEnum));
            var exp = Expression.Lambda<Func<TestEnum, bool>>(Expression.TypeEqual(parameter, typeof(Enum)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(TestEnum.One));
        }

        public class TestClassA
        {
        }

        public class TestClassB : TestClassA
        {
        }

        private enum TestEnum
        {
            One,
            Two
        }
    }
}