using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.LeftShiftAssign
{
    [TestFixture]
    public class TestStaticMember
    {
        [Test]
        public void Test1()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("IntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.IntProp = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.IntProp);
            TestClassA.IntProp = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.IntProp);
            TestClassA.IntProp = 1;
            Assert.AreEqual(1024, f(10));
            Assert.AreEqual(1024, TestClassA.IntProp);
            TestClassA.IntProp = 1234;
            Assert.AreEqual(2468, f(1));
            Assert.AreEqual(2468, TestClassA.IntProp);
            TestClassA.IntProp = 1;
            Assert.AreEqual(16, f(100));
            Assert.AreEqual(16, TestClassA.IntProp);
            TestClassA.IntProp = -1234;
            Assert.AreEqual(-2468, f(1));
            Assert.AreEqual(-2468, TestClassA.IntProp);
        }

        [Test]
        public void Test2()
        {
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.NullableIntField = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1;
            Assert.AreEqual(1024, f(10));
            Assert.AreEqual(1024, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1234;
            Assert.AreEqual(2468, f(1));
            Assert.AreEqual(2468, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1;
            Assert.AreEqual(16, f(100));
            Assert.AreEqual(16, TestClassA.NullableIntField);
            TestClassA.NullableIntField = -1234;
            Assert.AreEqual(-2468, f(1));
            Assert.AreEqual(-2468, TestClassA.NullableIntField);
            TestClassA.NullableIntField = null;
            Assert.IsNull(f(2));
            Assert.IsNull(TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1;
            Assert.IsNull(f(null));
            Assert.IsNull(TestClassA.NullableIntField);
            Assert.IsNull(f(null));
            Assert.IsNull(TestClassA.NullableIntField);
        }

        [Test]
        public void Test3()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int?>> exp = Expression.Lambda<Func<int, int?>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.NullableIntField = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1;
            Assert.AreEqual(1024, f(10));
            Assert.AreEqual(1024, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1234;
            Assert.AreEqual(2468, f(1));
            Assert.AreEqual(2468, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1;
            Assert.AreEqual(16, f(100));
            Assert.AreEqual(16, TestClassA.NullableIntField);
            TestClassA.NullableIntField = -1234;
            Assert.AreEqual(-2468, f(1));
            Assert.AreEqual(-2468, TestClassA.NullableIntField);
            TestClassA.NullableIntField = null;
            Assert.IsNull(f(2));
            Assert.IsNull(TestClassA.NullableIntField);
        }

        [Test]
        public void Test4()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, uint>> exp = Expression.Lambda<Func<int, uint>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("UIntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.UIntProp = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.UIntProp);
            TestClassA.UIntProp = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.UIntProp);
            TestClassA.UIntProp = 1;
            Assert.AreEqual(1024, f(10));
            Assert.AreEqual(1024, TestClassA.UIntProp);
            TestClassA.UIntProp = 1234;
            Assert.AreEqual(2468, f(1));
            Assert.AreEqual(2468, TestClassA.UIntProp);
            TestClassA.UIntProp = 1;
            Assert.AreEqual(16, f(100));
            Assert.AreEqual(16, TestClassA.UIntProp);
            TestClassA.UIntProp = 1;
            Assert.AreEqual(2147483648, f(31));
            Assert.AreEqual(2147483648, TestClassA.UIntProp);
        }

        [Test]
        public void Test5()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, uint?>> exp = Expression.Lambda<Func<int, uint?>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableUIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.NullableUIntField = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1;
            Assert.AreEqual(1024, f(10));
            Assert.AreEqual(1024, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1234;
            Assert.AreEqual(2468, f(1));
            Assert.AreEqual(2468, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1;
            Assert.AreEqual(16, f(100));
            Assert.AreEqual(16, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1;
            Assert.AreEqual(2147483648, f(31));
            Assert.AreEqual(2147483648, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = null;
            Assert.IsNull(f(2));
            Assert.IsNull(TestClassA.NullableUIntField);
        }

        [Test]
        public void Test6()
        {
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, uint?>> exp = Expression.Lambda<Func<int?, uint?>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableUIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.NullableUIntField = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1;
            Assert.AreEqual(1024, f(10));
            Assert.AreEqual(1024, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1234;
            Assert.AreEqual(2468, f(1));
            Assert.AreEqual(2468, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1;
            Assert.AreEqual(16, f(100));
            Assert.AreEqual(16, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1;
            Assert.AreEqual(2147483648, f(31));
            Assert.AreEqual(2147483648, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = null;
            Assert.IsNull(f(2));
            Assert.IsNull(TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1;
            Assert.IsNull(f(null));
            Assert.IsNull(TestClassA.NullableUIntField);
            Assert.IsNull(f(null));
            Assert.IsNull(TestClassA.NullableUIntField);
        }

        public class TestClassA
        {
            public static int IntProp { get; set; }
            public static uint UIntProp { get; set; }
            public static int? NullableIntField;
            public static uint? NullableUIntField;
        }
    }
}