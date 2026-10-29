using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.ExclusiveOrAssign
{
    [TestFixture]
    public class TestSimpleArray
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.ExclusiveOrAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new[] {0}};
            ClassicAssert.AreEqual(123, f(o, 123));
            ClassicAssert.AreEqual(123, o.IntArray[0]);
            o.IntArray[0] = 5;
            ClassicAssert.AreEqual(6, f(o, 3));
            ClassicAssert.AreEqual(6, o.IntArray[0]);
            o.IntArray[0] = 17235476;
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(o, 73172563));
            ClassicAssert.AreEqual(17235476 ^ 73172563, o.IntArray[0]);
            ClassicAssert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new[] {0}};
            ClassicAssert.AreEqual(123, f(o, 123));
            ClassicAssert.AreEqual(123, o.IntArray[0]);
            o.IntArray[0] = 5;
            ClassicAssert.AreEqual(6, f(o, 3));
            ClassicAssert.AreEqual(6, o.IntArray[0]);
            o.IntArray[0] = 17235476;
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(o, 73172563));
            ClassicAssert.AreEqual(17235476 ^ 73172563, o.IntArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.ExclusiveOrAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntArray")), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new int?[] {0}};
            ClassicAssert.AreEqual(123, f(o, 123));
            ClassicAssert.AreEqual(123, o.NullableIntArray[0]);
            o.NullableIntArray[0] = 5;
            ClassicAssert.AreEqual(6, f(o, 3));
            ClassicAssert.AreEqual(6, o.NullableIntArray[0]);
            o.NullableIntArray[0] = 17235476;
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(o, 73172563));
            ClassicAssert.AreEqual(17235476 ^ 73172563, o.NullableIntArray[0]);
            ClassicAssert.IsNull(f(null, 1));
            o.NullableIntArray[0] = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableIntArray[0]);
            o.NullableIntArray[0] = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntArray[0]);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntArray[0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new int?[] {0}};
            ClassicAssert.AreEqual(123, f(o, 123));
            ClassicAssert.AreEqual(123, o.NullableIntArray[0]);
            o.NullableIntArray[0] = 5;
            ClassicAssert.AreEqual(6, f(o, 3));
            ClassicAssert.AreEqual(6, o.NullableIntArray[0]);
            o.NullableIntArray[0] = 17235476;
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(o, 73172563));
            ClassicAssert.AreEqual(17235476 ^ 73172563, o.NullableIntArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntArray[0] = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableIntArray[0]);
            o.NullableIntArray[0] = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntArray[0]);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntArray[0]);
        }

        public class TestClassA
        {
            public int[] IntArray { get; set; }
            public int?[] NullableIntArray;
        }
    }
}