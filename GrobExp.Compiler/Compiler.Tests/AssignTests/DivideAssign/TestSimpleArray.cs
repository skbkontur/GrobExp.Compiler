using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.DivideAssign
{
    [TestFixture]
    public class TestSimpleArray
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.DivideAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new[] {1}};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.IntArray[0]);
            o.IntArray[0] = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntArray[0]);
            o.IntArray[0] = -3;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.IntArray[0]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new[] {1}};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.IntArray[0]);
            o.IntArray[0] = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntArray[0]);
            o.IntArray[0] = -3;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.IntArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.DivideAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[] {1}};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.NullableIntArray[0]);
            o.NullableIntArray[0] = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntArray[0]);
            o.NullableIntArray[0] = -3;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.NullableIntArray[0]);
            Assert.IsNull(f(null, 1));
            o.NullableIntArray[0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray[0]);
            o.NullableIntArray[0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new int?[] {1}};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.NullableIntArray[0]);
            o.NullableIntArray[0] = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntArray[0]);
            o.NullableIntArray[0] = -3;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.NullableIntArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntArray[0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray[0]);
            o.NullableIntArray[0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0]);
        }

        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<TestClassA, double, double>> exp = Expression.Lambda<Func<TestClassA, double, double>>(Expression.DivideAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleArray")), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleArray = new[] {1.0}};
            Assert.AreEqual(0.5, f(o, 2));
            Assert.AreEqual(0.5, o.DoubleArray[0]);
            o.DoubleArray[0] = 5;
            Assert.AreEqual(2.5, f(o, 2));
            Assert.AreEqual(2.5, o.DoubleArray[0]);
            o.DoubleArray[0] = -3;
            Assert.AreEqual(-1.5, f(o, 2));
            Assert.AreEqual(-1.5, o.DoubleArray[0]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleArray = new[] {1.0}};
            Assert.AreEqual(0.5, f(o, 2));
            Assert.AreEqual(0.5, o.DoubleArray[0]);
            o.DoubleArray[0] = 5;
            Assert.AreEqual(2.5, f(o, 2));
            Assert.AreEqual(2.5, o.DoubleArray[0]);
            o.DoubleArray[0] = -3;
            Assert.AreEqual(-1.5, f(o, 2));
            Assert.AreEqual(-1.5, o.DoubleArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestUInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<TestClassA, uint, uint>> exp = Expression.Lambda<Func<TestClassA, uint, uint>>(Expression.DivideAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("UIntArray")), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntArray = new[] {1U}};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.UIntArray[0]);
            o.UIntArray[0] = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.UIntArray[0]);
            o.UIntArray[0] = uint.MaxValue - 3 + 1;
            Assert.AreEqual(2147483646, f(o, 2));
            Assert.AreEqual(2147483646, o.UIntArray[0]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntArray = new[] {1U}};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.UIntArray[0]);
            o.UIntArray[0] = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.UIntArray[0]);
            o.UIntArray[0] = uint.MaxValue - 3 + 1;
            Assert.AreEqual(2147483646, f(o, 2));
            Assert.AreEqual(2147483646, o.UIntArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        public class TestClassA
        {
            public int[] IntArray { get; set; }
            public int?[] NullableIntArray { get; set; }
            public double[] DoubleArray;
            public uint[] UIntArray;
        }
    }
}