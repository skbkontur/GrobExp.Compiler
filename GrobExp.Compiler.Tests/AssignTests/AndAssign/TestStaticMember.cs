using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.AndAssign
{
    [TestFixture]
    public class TestStaticMember
    {
        [Test]
        public void TestIntProp()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.AndAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("IntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.IntProp = 0;
            ClassicAssert.AreEqual(0, f(123));
            ClassicAssert.AreEqual(0, TestClassA.IntProp);
            TestClassA.IntProp = 5;
            ClassicAssert.AreEqual(1, f(3));
            ClassicAssert.AreEqual(1, TestClassA.IntProp);
            TestClassA.IntProp = 17235476;
            ClassicAssert.AreEqual(17235476 & 73172563, f(73172563));
            ClassicAssert.AreEqual(17235476 & 73172563, TestClassA.IntProp);
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.AndAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            TestClassA.NullableIntField = 0;
            ClassicAssert.AreEqual(0, f(123));
            ClassicAssert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 5;
            ClassicAssert.AreEqual(1, f(3));
            ClassicAssert.AreEqual(1, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 17235476;
            ClassicAssert.AreEqual(17235476 & 73172563, f(73172563));
            ClassicAssert.AreEqual(17235476 & 73172563, TestClassA.NullableIntField);
            TestClassA.NullableIntField = null;
            ClassicAssert.IsNull(f(2));
            ClassicAssert.IsNull(TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1;
            ClassicAssert.IsNull(f(null));
            ClassicAssert.IsNull(TestClassA.NullableIntField);
            ClassicAssert.IsNull(f(null));
            ClassicAssert.IsNull(TestClassA.NullableIntField);
        }

        public class TestClassA
        {
            public static int IntProp { get; set; }
            public static int? NullableIntField;
        }
    }
}