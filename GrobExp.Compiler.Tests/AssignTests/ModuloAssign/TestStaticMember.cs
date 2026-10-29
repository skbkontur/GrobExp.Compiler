using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.ModuloAssign
{
    [TestFixture]
    public class TestStaticMember
    {
        [Test]
        public void TestIntProp()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.ModuloAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("IntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.IntProp = 1;
            ClassicAssert.AreEqual(1, f(2));
            ClassicAssert.AreEqual(1, TestClassA.IntProp);
            TestClassA.IntProp = 5;
            ClassicAssert.AreEqual(2, f(3));
            ClassicAssert.AreEqual(2, TestClassA.IntProp);
            TestClassA.IntProp = -3;
            ClassicAssert.AreEqual(-1, f(2));
            ClassicAssert.AreEqual(-1, TestClassA.IntProp);
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.ModuloAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("NullableIntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            TestClassA.NullableIntProp = 1;
            ClassicAssert.AreEqual(1, f(2));
            ClassicAssert.AreEqual(1, TestClassA.NullableIntProp);
            TestClassA.NullableIntProp = 5;
            ClassicAssert.AreEqual(2, f(3));
            ClassicAssert.AreEqual(2, TestClassA.NullableIntProp);
            TestClassA.NullableIntProp = -3;
            ClassicAssert.AreEqual(-1, f(2));
            ClassicAssert.AreEqual(-1, TestClassA.NullableIntProp);
            TestClassA.NullableIntProp = null;
            ClassicAssert.IsNull(f(2));
            ClassicAssert.IsNull(TestClassA.NullableIntProp);
            TestClassA.NullableIntProp = 1;
            ClassicAssert.IsNull(f(null));
            ClassicAssert.IsNull(TestClassA.NullableIntProp);
            ClassicAssert.IsNull(f(null));
            ClassicAssert.IsNull(TestClassA.NullableIntProp);
        }

        [Test]
        public void TestUInt()
        {
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<uint, uint>> exp = Expression.Lambda<Func<uint, uint>>(Expression.ModuloAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("UIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.UIntField = 1;
            ClassicAssert.AreEqual(1, f(2));
            ClassicAssert.AreEqual(1, TestClassA.UIntField);
            TestClassA.UIntField = 5;
            ClassicAssert.AreEqual(2, f(3));
            ClassicAssert.AreEqual(2, TestClassA.UIntField);
            TestClassA.UIntField = uint.MaxValue - 3 + 1;
            ClassicAssert.AreEqual(1, f(2));
            ClassicAssert.AreEqual(1, TestClassA.UIntField);
        }

        public class TestClassA
        {
            public static int IntProp { get; set; }
            public static int? NullableIntProp { get; set; }
            public static uint UIntField;
        }
    }
}