using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.PostDecrementAssign
{
    [TestFixture]
    public class TestInstanceMember
    {
        [Test]
        public void TestIntProp()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(Expression.PostDecrementAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntProp"))), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntProp = 0};
            ClassicAssert.AreEqual(0, f(o));
            ClassicAssert.AreEqual(-1, o.IntProp);
            o.IntProp = 1;
            ClassicAssert.AreEqual(1, f(o));
            ClassicAssert.AreEqual(0, o.IntProp);
            o.IntProp = int.MinValue;
            ClassicAssert.AreEqual(int.MinValue, f(o));
            ClassicAssert.AreEqual(int.MaxValue, o.IntProp);
            ClassicAssert.AreEqual(0, f(null));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntProp = 0};
            ClassicAssert.AreEqual(0, f(o));
            ClassicAssert.AreEqual(-1, o.IntProp);
            o.IntProp = 1;
            ClassicAssert.AreEqual(1, f(o));
            ClassicAssert.AreEqual(0, o.IntProp);
            o.IntProp = int.MinValue;
            ClassicAssert.AreEqual(int.MinValue, f(o));
            ClassicAssert.AreEqual(int.MaxValue, o.IntProp);
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        [Test]
        public void TestDoubleField()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, double>> exp = Expression.Lambda<Func<TestClassA, double>>(Expression.PostDecrementAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleField"))), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleField = 0};
            ClassicAssert.AreEqual(0, f(o));
            ClassicAssert.AreEqual(-1, o.DoubleField);
            o.DoubleField = 1;
            ClassicAssert.AreEqual(1, f(o));
            ClassicAssert.AreEqual(0, o.DoubleField);
            o.DoubleField = 0.5;
            ClassicAssert.AreEqual(0.5, f(o));
            ClassicAssert.AreEqual(-0.5, o.DoubleField);
            ClassicAssert.AreEqual(0, f(null));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleField = 0};
            ClassicAssert.AreEqual(0, f(o));
            ClassicAssert.AreEqual(-1, o.DoubleField);
            o.DoubleField = 1;
            ClassicAssert.AreEqual(1, f(o));
            ClassicAssert.AreEqual(0, o.DoubleField);
            o.DoubleField = 0.5;
            ClassicAssert.AreEqual(0.5, f(o));
            ClassicAssert.AreEqual(-0.5, o.DoubleField);
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, int?>> exp = Expression.Lambda<Func<TestClassA, int?>>(Expression.PostDecrementAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntProp"))), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntProp = 0};
            ClassicAssert.AreEqual(0, f(o));
            ClassicAssert.AreEqual(-1, o.NullableIntProp);
            o.NullableIntProp = 1;
            ClassicAssert.AreEqual(1, f(o));
            ClassicAssert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = int.MinValue;
            ClassicAssert.AreEqual(int.MinValue, f(o));
            ClassicAssert.AreEqual(int.MaxValue, o.NullableIntProp);
            ClassicAssert.IsNull(f(null));
            o.NullableIntProp = null;
            ClassicAssert.IsNull(f(o));
            ClassicAssert.IsNull(o.NullableIntProp);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntProp = 0};
            ClassicAssert.AreEqual(0, f(o));
            ClassicAssert.AreEqual(-1, o.NullableIntProp);
            o.NullableIntProp = 1;
            ClassicAssert.AreEqual(1, f(o));
            ClassicAssert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = int.MinValue;
            ClassicAssert.AreEqual(int.MinValue, f(o));
            ClassicAssert.AreEqual(int.MaxValue, o.NullableIntProp);
            Assert.Throws<NullReferenceException>(() => f(null));
            o.NullableIntProp = null;
            ClassicAssert.IsNull(f(o));
            ClassicAssert.IsNull(o.NullableIntProp);
        }

        public class TestClassA
        {
            public int IntProp { get; set; }
            public int? NullableIntProp { get; set; }
            public double DoubleField;
        }
    }
}