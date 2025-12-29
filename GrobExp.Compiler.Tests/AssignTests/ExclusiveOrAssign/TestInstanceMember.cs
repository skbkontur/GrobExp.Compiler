using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.ExclusiveOrAssign
{
    [TestFixture]
    public class TestInstanceMember
    {
        [Test]
        public void TestIntProp()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.ExclusiveOrAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntProp = 0};
            ClassicAssert.AreEqual(123, f(o, 123));
            ClassicAssert.AreEqual(123, o.IntProp);
            o.IntProp = 3;
            ClassicAssert.AreEqual(6, f(o, 5));
            ClassicAssert.AreEqual(6, o.IntProp);
            o.IntProp = 17235476;
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(o, 73172563));
            ClassicAssert.AreEqual(17235476 ^ 73172563, o.IntProp);
            ClassicAssert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntProp = 0};
            ClassicAssert.AreEqual(123, f(o, 123));
            ClassicAssert.AreEqual(123, o.IntProp);
            o.IntProp = 3;
            ClassicAssert.AreEqual(6, f(o, 5));
            ClassicAssert.AreEqual(6, o.IntProp);
            o.IntProp = 17235476;
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(o, 73172563));
            ClassicAssert.AreEqual(17235476 ^ 73172563, o.IntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullableIntField()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.ExclusiveOrAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntField = 0};
            ClassicAssert.AreEqual(123, f(o, 123));
            ClassicAssert.AreEqual(123, o.NullableIntField);
            o.NullableIntField = 3;
            ClassicAssert.AreEqual(6, f(o, 5));
            ClassicAssert.AreEqual(6, o.NullableIntField);
            o.NullableIntField = 17235476;
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(o, 73172563));
            ClassicAssert.AreEqual(17235476 ^ 73172563, o.NullableIntField);
            ClassicAssert.IsNull(f(null, 1));
            o.NullableIntField = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableIntField);
            o.NullableIntField = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntField);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntField);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntField = 0};
            ClassicAssert.AreEqual(123, f(o, 123));
            ClassicAssert.AreEqual(123, o.NullableIntField);
            o.NullableIntField = 3;
            ClassicAssert.AreEqual(6, f(o, 5));
            ClassicAssert.AreEqual(6, o.NullableIntField);
            o.NullableIntField = 17235476;
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(o, 73172563));
            ClassicAssert.AreEqual(17235476 ^ 73172563, o.NullableIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntField = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableIntField);
            o.NullableIntField = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntField);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntField);
        }

        public class TestClassA
        {
            public int IntProp { get; set; }
            public int? NullableIntField;
        }
    }
}