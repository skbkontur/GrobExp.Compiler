using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PostDecrementAssign
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
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(-1, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = int.MinValue;
            Assert.AreEqual(int.MinValue, f(o));
            Assert.AreEqual(int.MaxValue, o.IntProp);
            Assert.AreEqual(0, f(null));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntProp = 0};
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(-1, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = int.MinValue;
            Assert.AreEqual(int.MinValue, f(o));
            Assert.AreEqual(int.MaxValue, o.IntProp);
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        [Test]
        public void TestDoubleField()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, double>> exp = Expression.Lambda<Func<TestClassA, double>>(Expression.PostDecrementAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleField"))), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleField = 0};
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(-1, o.DoubleField);
            o.DoubleField = 1;
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(0, o.DoubleField);
            o.DoubleField = 0.5;
            Assert.AreEqual(0.5, f(o));
            Assert.AreEqual(-0.5, o.DoubleField);
            Assert.AreEqual(0, f(null));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleField = 0};
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(-1, o.DoubleField);
            o.DoubleField = 1;
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(0, o.DoubleField);
            o.DoubleField = 0.5;
            Assert.AreEqual(0.5, f(o));
            Assert.AreEqual(-0.5, o.DoubleField);
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, int?>> exp = Expression.Lambda<Func<TestClassA, int?>>(Expression.PostDecrementAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntProp"))), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntProp = 0};
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(-1, o.NullableIntProp);
            o.NullableIntProp = 1;
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = int.MinValue;
            Assert.AreEqual(int.MinValue, f(o));
            Assert.AreEqual(int.MaxValue, o.NullableIntProp);
            Assert.IsNull(f(null));
            o.NullableIntProp = null;
            Assert.IsNull(f(o));
            Assert.IsNull(o.NullableIntProp);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntProp = 0};
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(-1, o.NullableIntProp);
            o.NullableIntProp = 1;
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = int.MinValue;
            Assert.AreEqual(int.MinValue, f(o));
            Assert.AreEqual(int.MaxValue, o.NullableIntProp);
            Assert.Throws<NullReferenceException>(() => f(null));
            o.NullableIntProp = null;
            Assert.IsNull(f(o));
            Assert.IsNull(o.NullableIntProp);
        }

        public class TestClassA
        {
            public int IntProp { get; set; }
            public int? NullableIntProp { get; set; }
            public double DoubleField;
        }
    }
}