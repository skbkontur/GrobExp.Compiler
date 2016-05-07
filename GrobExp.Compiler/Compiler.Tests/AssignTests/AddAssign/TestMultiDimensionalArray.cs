using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.AddAssign
{
    [TestFixture]
    public class TestMultiDimensionalArray
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.AddAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 + 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 + 2000000000, o.IntArray[0, 0]);
            }
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 + 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 + 2000000000, o.IntArray[0, 0]);
            }
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestIntx()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            BinaryExpression addAssign = Expression.AddAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0), Expression.Constant(0)), b);
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.Block(addAssign, Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0), Expression.Constant(0))), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 + 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 + 2000000000, o.IntArray[0, 0]);
            }
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            var f2 = exp.Compile();
            Assert.IsNotNull(f2);
            o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 + 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 + 2000000000, o.IntArray[0, 0]);
            }
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.AddAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 + 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 + 2000000000, o.NullableIntArray[0, 0]);
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
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 + 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 + 2000000000, o.NullableIntArray[0, 0]);
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
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.AddAssignChecked(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new int[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestCheckedNullableSigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.AddAssignChecked(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
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
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
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
            Expression<Func<TestClassA, uint, uint>> exp = Expression.Lambda<Func<TestClassA, uint, uint>>(Expression.AddAssignChecked(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("UIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntArray = new uint[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1000000000;
            Assert.AreEqual(3000000000, f(o, 2000000000));
            Assert.AreEqual(3000000000, o.UIntArray[0, 0]);
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntArray = new uint[1,1]};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1000000000;
            Assert.AreEqual(3000000000, f(o, 2000000000));
            Assert.AreEqual(3000000000, o.UIntArray[0, 0]);
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestCheckedNullableUnsigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint?), "b");
            Expression<Func<TestClassA, uint?, uint?>> exp = Expression.Lambda<Func<TestClassA, uint?, uint?>>(Expression.AddAssignChecked(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntArray = new uint?[1,1]};
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1;
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1000000000;
            Assert.AreEqual(3000000000, f(o, 2000000000));
            Assert.AreEqual(3000000000, o.NullableUIntArray[0, 0]);
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
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
            Assert.AreEqual(3, f(o, 2));
            Assert.AreEqual(3, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1000000000;
            Assert.AreEqual(3000000000, f(o, 2000000000));
            Assert.AreEqual(3000000000, o.NullableUIntArray[0, 0]);
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
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