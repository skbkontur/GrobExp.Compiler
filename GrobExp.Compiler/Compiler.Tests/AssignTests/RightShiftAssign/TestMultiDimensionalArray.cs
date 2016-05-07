using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.RightShiftAssign
{
    [TestFixture]
    public class TestMultiDimensionalArray
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.RightShiftAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new int[1,1]};
            o.IntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -3;
            Assert.AreEqual(-2, f(o, 1));
            Assert.AreEqual(-2, o.IntArray[0, 0]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new int[1,1]};
            o.IntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.IntArray[0, 0]);
            o.IntArray[0, 0] = -3;
            Assert.AreEqual(-2, f(o, 1));
            Assert.AreEqual(-2, o.IntArray[0, 0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.RightShiftAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -3;
            Assert.AreEqual(-2, f(o, 1));
            Assert.AreEqual(-2, o.NullableIntArray[0, 0]);
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
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -3;
            Assert.AreEqual(-2, f(o, 1));
            Assert.AreEqual(-2, o.NullableIntArray[0, 0]);
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
        public void Test3()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int?>> exp = Expression.Lambda<Func<TestClassA, int, int?>>(Expression.RightShiftAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -3;
            Assert.AreEqual(-2, f(o, 1));
            Assert.AreEqual(-2, o.NullableIntArray[0, 0]);
            Assert.IsNull(f(null, 1));
            o.NullableIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray[0, 0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = -3;
            Assert.AreEqual(-2, f(o, 1));
            Assert.AreEqual(-2, o.NullableIntArray[0, 0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray[0, 0]);
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, uint>> exp = Expression.Lambda<Func<TestClassA, int, uint>>(Expression.RightShiftAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("UIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntArray = new uint[1,1]};
            o.UIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 4000000000;
            Assert.AreEqual(2000000000, f(o, 1));
            Assert.AreEqual(2000000000, o.UIntArray[0, 0]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntArray = new uint[1,1]};
            o.UIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.UIntArray[0, 0]);
            o.UIntArray[0, 0] = 4000000000;
            Assert.AreEqual(2000000000, f(o, 1));
            Assert.AreEqual(2000000000, o.UIntArray[0, 0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, uint?>> exp = Expression.Lambda<Func<TestClassA, int, uint?>>(Expression.RightShiftAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntArray = new uint?[1,1]};
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 4000000000;
            Assert.AreEqual(2000000000, f(o, 1));
            Assert.AreEqual(2000000000, o.NullableUIntArray[0, 0]);
            Assert.IsNull(f(null, 1));
            o.NullableUIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntArray[0, 0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableUIntArray = new uint?[1,1]};
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 4000000000;
            Assert.AreEqual(2000000000, f(o, 1));
            Assert.AreEqual(2000000000, o.NullableUIntArray[0, 0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableUIntArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntArray[0, 0]);
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, uint?>> exp = Expression.Lambda<Func<TestClassA, int?, uint?>>(Expression.RightShiftAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntArray = new uint?[1,1]};
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 4000000000;
            Assert.AreEqual(2000000000, f(o, 1));
            Assert.AreEqual(2000000000, o.NullableUIntArray[0, 0]);
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
            o.NullableUIntArray[0, 0] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1024;
            Assert.AreEqual(1, f(o, 10));
            Assert.AreEqual(1, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 1023;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 3;
            Assert.AreEqual(1, f(o, 1));
            Assert.AreEqual(1, o.NullableUIntArray[0, 0]);
            o.NullableUIntArray[0, 0] = 4000000000;
            Assert.AreEqual(2000000000, f(o, 1));
            Assert.AreEqual(2000000000, o.NullableUIntArray[0, 0]);
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
            public uint[,] UIntArray { get; set; }
            public int?[,] NullableIntArray;
            public uint?[,] NullableUIntArray;
        }
    }
}