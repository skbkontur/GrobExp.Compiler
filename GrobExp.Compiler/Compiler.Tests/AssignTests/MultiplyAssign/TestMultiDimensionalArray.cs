using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.MultiplyAssign
{
    [TestFixture]
    public class TestMultiDimensionalArray
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.MultiplyAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.IntArray[0, 0]);
            }
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.IntArray[0, 0]);
            }
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.MultiplyAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.NullableIntArray[0, 0]);
            }
            Assert.IsNull(f(null, 1));
            o.NullableIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0, 0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0, 0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.NullableIntArray[0, 0]);
            }
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0, 0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0, 0]);
        }

        [Test]
        public void TestCheckedSigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.MultiplyAssignChecked(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestCheckedNullableSigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.MultiplyAssignChecked(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.IsNull(f(null, 1));
            o.NullableIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0, 0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0, 0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0, 0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray[0, 0]);
        }

        [Test]
        public void TestCheckedUnsigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<TestClassA, uint, uint>> exp = Expression.Lambda<Func<TestClassA, uint, uint>>(Expression.MultiplyAssignChecked(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("UIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntArray = new uint[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 2000000000;
            Assert.AreEqual(4000000000, f(o, 2));
            Assert.AreEqual(4000000000, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 3));
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntArray = new uint[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 2000000000;
            Assert.AreEqual(4000000000, f(o, 2));
            Assert.AreEqual(4000000000, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 3));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestCheckedNullableUnsigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint?), "b");
            Expression<Func<TestClassA, uint?, uint?>> exp = Expression.Lambda<Func<TestClassA, uint?, uint?>>(Expression.MultiplyAssignChecked(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntArray = new uint?[1,1]};
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 2000000000;
            Assert.AreEqual(4000000000, f(o, 2));
            Assert.AreEqual(4000000000, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 3));
            Assert.IsNull(f(null, 1));
            o.NullableUIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntArray[0, 0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntArray[0, 0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableUIntArray = new uint?[1,1]};
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 2000000000;
            Assert.AreEqual(4000000000, f(o, 2));
            Assert.AreEqual(4000000000, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 3));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableUIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntArray[0, 0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntArray[0, 0]);
        }

        public class TestClassA
        {
            public int[,] IntArray { get; set; }
            public int?[,] NullableIntArray { get; set; }
            public uint[,] UIntArray;
            public uint?[,] NullableUIntArray;
        }
    }
}