using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestBlock
    {
        [Test]
        public void TestDroppingResult1()
        {
            Expression<Func<long, DateTime>> exp = ticks => new DateTime(ticks);
            Expression<Func<long, long>> exp2 = Expression.Lambda<Func<long, long>>(Expression.Block(typeof(long), exp.Body, exp.Parameters[0]), exp.Parameters);
            var f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            Assert.AreEqual(1234566789123456789, f(1234566789123456789));
        }

        [Test]
        public void TestDroppingResult2()
        {
            Expression<Func<long, DateTime>> exp = ticks => new DateTime(ticks);
            Expression<Action<long>> exp2 = Expression.Lambda<Action<long>>(Expression.Block(typeof(void), exp.Body, exp.Parameters[0]), exp.Parameters);
            var f = LambdaCompiler.Compile(exp2,CompilerOptions.All);
            f(123485234785);
        }

        [Test]
        public void TestDroppingResult3()
        {
            Expression<Func<long, DateTime>> exp = ticks => new DateTime(ticks);
            Expression<Action<long>> exp2 = Expression.Lambda<Action<long>>(Expression.Block(typeof(void), exp.Body), exp.Parameters);
            var f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            f(123485234785);
        }

        [Test]
        public void TestReturnsBoolCorrectly()
        {
            Expression<Func<TestClassA, bool>> exp = a => a.X > 0;
            Expression<Func<TestClassA, bool>> exp2 = Expression.Lambda<Func<TestClassA, bool>>(Expression.Block(typeof(bool), exp.Body), exp.Parameters);
            var f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            Assert.AreEqual(false, f(null));
            Assert.AreEqual(false, f(new TestClassA()));
            Assert.AreEqual(false, f(new TestClassA {X = -1}));
            Assert.AreEqual(true, f(new TestClassA {X = 1}));
        }

        public class TestClassA
        {
            public int F(bool b)
            {
                return b ? 1 : 0;
            }

            public string S { get; set; }
            public TestClassA A { get; set; }
            public TestClassB B { get; set; }
            public TestClassB[] ArrayB { get; set; }
            public int[] IntArray { get; set; }
            public int? X;
            public Guid Guid = Guid.Empty;
            public Guid? NullableGuid;
            public bool? NullableBool;
            public int Y;
            public bool Bool;
            public TestStructA structA;
        }

        public class TestClassB
        {
            public int? F2(int? x)
            {
                return x;
            }

            public int? F( /*Qzz*/ int a, int b)
            {
                return b;
            }

            public string S { get; set; }

            public TestClassC C { get; set; }
            public int? X;
            public int Y;
        }

        public class TestClassC
        {
            public string S { get; set; }

            public TestClassD D { get; set; }

            public TestClassD[] ArrayD { get; set; }
        }

        public class TestClassD
        {
            public TestClassE E { get; set; }
            public TestClassE[] ArrayE { get; set; }
            public string Z { get; set; }

            public int? X { get; set; }

            public readonly string S;
        }

        public class TestClassE
        {
            public string S { get; set; }
            public int X { get; set; }
        }

        public struct TestStructA
        {
            public string S { get; set; }
            public int? X { get; set; }
            public int Y { get; set; }
            public TestStructB b;
        }

        public struct TestStructB
        {
            public string S { get; set; }
        }

        public struct Qzz
        {
            public long X;
        }
    }
}