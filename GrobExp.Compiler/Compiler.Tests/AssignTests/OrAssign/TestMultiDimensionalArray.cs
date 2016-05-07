using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.OrAssign
{
    [TestFixture]
    public class TestMultiDimensionalArray
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.OrAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new int[1,1]};
            o.IntArray[0, 0] = 0;
            Assert.AreEqual(123, f(o, 123));
            Assert.AreEqual(123, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 5;
            Assert.AreEqual(7, f(o, 3));
            Assert.AreEqual(7, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 17235476;
            Assert.AreEqual(17235476 | 73172563, f(o, 73172563));
            Assert.AreEqual(17235476 | 73172563, o.IntArray[0, 0]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new int[1,1]};
            o.IntArray[0, 0] = 0;
            Assert.AreEqual(123, f(o, 123));
            Assert.AreEqual(123, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 5;
            Assert.AreEqual(7, f(o, 3));
            Assert.AreEqual(7, o.IntArray[0, 0]);
            o.IntArray[0, 0] = 17235476;
            Assert.AreEqual(17235476 | 73172563, f(o, 73172563));
            Assert.AreEqual(17235476 | 73172563, o.IntArray[0, 0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.OrAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[1,1]};
            o.NullableIntArray[0, 0] = 0;
            Assert.AreEqual(123, f(o, 123));
            Assert.AreEqual(123, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 5;
            Assert.AreEqual(7, f(o, 3));
            Assert.AreEqual(7, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 17235476;
            Assert.AreEqual(17235476 | 73172563, f(o, 73172563));
            Assert.AreEqual(17235476 | 73172563, o.NullableIntArray[0, 0]);
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
            Assert.AreEqual(123, f(o, 123));
            Assert.AreEqual(123, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 5;
            Assert.AreEqual(7, f(o, 3));
            Assert.AreEqual(7, o.NullableIntArray[0, 0]);
            o.NullableIntArray[0, 0] = 17235476;
            Assert.AreEqual(17235476 | 73172563, f(o, 73172563));
            Assert.AreEqual(17235476 | 73172563, o.NullableIntArray[0, 0]);
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

        public class TestClassA
        {
            public int[,] IntArray { get; set; }
            public int?[,] NullableIntArray;
        }
    }
}