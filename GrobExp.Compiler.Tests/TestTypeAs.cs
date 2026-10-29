using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests
{
    [TestFixture]
    public class TestTypeAs
    {
        [Test]
        public void Test1()
        {
            var parameter = Expression.Parameter(typeof(int));
            var exp = Expression.Lambda<Func<int, double?>>(Expression.TypeAs(parameter, typeof(double?)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(f(5));
        }

        [Test]
        public void Test2()
        {
            var parameter = Expression.Parameter(typeof(int));
            var exp = Expression.Lambda<Func<int, int?>>(Expression.TypeAs(parameter, typeof(int?)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            int? x = f(5);
            Console.WriteLine(x.GetType());
            ClassicAssert.AreEqual(5, f(5));
        }

        [Test]
        public void Test3()
        {
            var parameter = Expression.Parameter(typeof(int));
            var exp = Expression.Lambda<Func<int, object>>(Expression.TypeAs(parameter, typeof(object)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            object res = f(5);
            ClassicAssert.AreEqual(5, res);
        }

        [Test]
        public void Test4()
        {
            var parameter = Expression.Parameter(typeof(TestClassB));
            var exp = Expression.Lambda<Func<TestClassB, TestClassA>>(Expression.TypeAs(parameter, typeof(TestClassA)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var b = new TestClassB();
            TestClassA a = f(b);
            ClassicAssert.AreEqual(b, a);
        }

        [Test]
        public void Test5()
        {
            var parameter = Expression.Parameter(typeof(TestClassA));
            var exp = Expression.Lambda<Func<TestClassA, TestClassB>>(Expression.TypeAs(parameter, typeof(TestClassB)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA();
            TestClassB b = f(a);
            ClassicAssert.IsNull(b);
            b = new TestClassB();
            var bb = f(b);
            ClassicAssert.AreEqual(b, bb);
        }

        [Test]
        public void Test6()
        {
            var parameter = Expression.Parameter(typeof(object));
            var exp = Expression.Lambda<Func<object, int?>>(Expression.TypeAs(parameter, typeof(int?)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(5, f(5));
            ClassicAssert.IsNull(f(5.5));
        }

        [Test]
        public void Test7()
        {
            var parameter = Expression.Parameter(typeof(object));
            var exp = Expression.Lambda<Func<object, TestEnum?>>(Expression.TypeAs(parameter, typeof(TestEnum?)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(TestEnum.One, f(TestEnum.One));
            ClassicAssert.IsNull(f(5.5));
        }

        [Test]
        public void Test8()
        {
            var parameter = Expression.Parameter(typeof(TestEnum));
            var exp = Expression.Lambda<Func<TestEnum, Enum>>(Expression.TypeAs(parameter, typeof(Enum)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Enum actual = f(TestEnum.One);
            ClassicAssert.AreEqual(TestEnum.One, actual);
        }

        public enum TestEnum
        {
            One,
            Two
        }

        public class TestClassA
        {
        }

        public class TestClassB : TestClassA
        {
        }
    }
}