using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PreIncrementAssign
{
    [TestFixture]
    public class TestSimpleArray
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(Expression.PreIncrementAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0))), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new[] {0}};
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(1, o.IntArray[0]);
            o.IntArray[0] = -1;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.IntArray[0]);
            o.IntArray[0] = int.MaxValue;
            Assert.AreEqual(int.MinValue, f(o));
            Assert.AreEqual(int.MinValue, o.IntArray[0]);
            Assert.AreEqual(0, f(null));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new[] {0}};
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(1, o.IntArray[0]);
            o.IntArray[0] = -1;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.IntArray[0]);
            o.IntArray[0] = int.MaxValue;
            Assert.AreEqual(int.MinValue, f(o));
            Assert.AreEqual(int.MinValue, o.IntArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, int?>> exp = Expression.Lambda<Func<TestClassA, int?>>(Expression.PreIncrementAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), Expression.Constant(0))), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[] {0}};
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(1, o.NullableIntArray[0]);
            o.NullableIntArray[0] = -1;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.NullableIntArray[0]);
            o.NullableIntArray[0] = int.MaxValue;
            Assert.AreEqual(int.MinValue, f(o));
            Assert.AreEqual(int.MinValue, o.NullableIntArray[0]);
            Assert.IsNull(f(null));
            o.NullableIntArray[0] = null;
            Assert.IsNull(f(o));
            Assert.IsNull(o.NullableIntArray[0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new int?[] {0}};
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(1, o.NullableIntArray[0]);
            o.NullableIntArray[0] = -1;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.NullableIntArray[0]);
            o.NullableIntArray[0] = int.MaxValue;
            Assert.AreEqual(int.MinValue, f(o));
            Assert.AreEqual(int.MinValue, o.NullableIntArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null));
            o.NullableIntArray[0] = null;
            Assert.IsNull(f(o));
            Assert.IsNull(o.NullableIntArray[0]);
        }

        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, double>> exp = Expression.Lambda<Func<TestClassA, double>>(Expression.PreIncrementAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleArray")), Expression.Constant(0))), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleArray = new[] {0.0}};
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(1, o.DoubleArray[0]);
            o.DoubleArray[0] = -1;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.DoubleArray[0]);
            o.DoubleArray[0] = -0.5;
            Assert.AreEqual(0.5, f(o));
            Assert.AreEqual(0.5, o.DoubleArray[0]);
            Assert.AreEqual(0, f(null));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleArray = new[] {0.0}};
            Assert.AreEqual(1, f(o));
            Assert.AreEqual(1, o.DoubleArray[0]);
            o.DoubleArray[0] = -1;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.DoubleArray[0]);
            o.DoubleArray[0] = -0.5;
            Assert.AreEqual(0.5, f(o));
            Assert.AreEqual(0.5, o.DoubleArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        public class TestClassA
        {
            public int[] IntArray { get; set; }
            public int?[] NullableIntArray { get; set; }
            public double[] DoubleArray;
        }
    }
}