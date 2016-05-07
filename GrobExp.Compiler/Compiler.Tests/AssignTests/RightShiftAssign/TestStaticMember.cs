using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.RightShiftAssign
{
    [TestFixture]
    public class TestStaticMember
    {
        [Test]
        public void Test1()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("IntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.IntProp = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.IntProp);
            TestClassA.IntProp = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.IntProp);
            TestClassA.IntProp = 1024;
            Assert.AreEqual(1, f(10));
            Assert.AreEqual(1, TestClassA.IntProp);
            TestClassA.IntProp = 1023;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.IntProp);
            TestClassA.IntProp = 3;
            Assert.AreEqual(1, f(1));
            Assert.AreEqual(1, TestClassA.IntProp);
            TestClassA.IntProp = -3;
            Assert.AreEqual(-2, f(1));
            Assert.AreEqual(-2, TestClassA.IntProp);
        }

        [Test]
        public void Test2()
        {
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.NullableIntField = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1024;
            Assert.AreEqual(1, f(10));
            Assert.AreEqual(1, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1023;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 3;
            Assert.AreEqual(1, f(1));
            Assert.AreEqual(1, TestClassA.NullableIntField);
            TestClassA.NullableIntField = -3;
            Assert.AreEqual(-2, f(1));
            Assert.AreEqual(-2, TestClassA.NullableIntField);
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
            Expression<Func<int, int?>> exp = Expression.Lambda<Func<int, int?>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.NullableIntField = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1024;
            Assert.AreEqual(1, f(10));
            Assert.AreEqual(1, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 1023;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableIntField);
            TestClassA.NullableIntField = 3;
            Assert.AreEqual(1, f(1));
            Assert.AreEqual(1, TestClassA.NullableIntField);
            TestClassA.NullableIntField = -3;
            Assert.AreEqual(-2, f(1));
            Assert.AreEqual(-2, TestClassA.NullableIntField);
            TestClassA.NullableIntField = null;
            Assert.IsNull(f(2));
            Assert.IsNull(TestClassA.NullableIntField);
        }

        [Test]
        public void Test4()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, uint>> exp = Expression.Lambda<Func<int, uint>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("UIntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.UIntProp = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.UIntProp);
            TestClassA.UIntProp = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.UIntProp);
            TestClassA.UIntProp = 1024;
            Assert.AreEqual(1, f(10));
            Assert.AreEqual(1, TestClassA.UIntProp);
            TestClassA.UIntProp = 1023;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.UIntProp);
            TestClassA.UIntProp = 3;
            Assert.AreEqual(1, f(1));
            Assert.AreEqual(1, TestClassA.UIntProp);
            TestClassA.UIntProp = 4000000000;
            Assert.AreEqual(2000000000, f(1));
            Assert.AreEqual(2000000000, TestClassA.UIntProp);
        }

        [Test]
        public void Test5()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, uint?>> exp = Expression.Lambda<Func<int, uint?>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableUIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.NullableUIntField = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1024;
            Assert.AreEqual(1, f(10));
            Assert.AreEqual(1, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1023;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 3;
            Assert.AreEqual(1, f(1));
            Assert.AreEqual(1, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 4000000000;
            Assert.AreEqual(2000000000, f(1));
            Assert.AreEqual(2000000000, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = null;
            Assert.IsNull(f(2));
            Assert.IsNull(TestClassA.NullableUIntField);
        }

        [Test]
        public void Test6()
        {
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, uint?>> exp = Expression.Lambda<Func<int?, uint?>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("NullableUIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.NullableUIntField = 0;
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 0;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1024;
            Assert.AreEqual(1, f(10));
            Assert.AreEqual(1, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 1023;
            Assert.AreEqual(0, f(10));
            Assert.AreEqual(0, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 3;
            Assert.AreEqual(1, f(1));
            Assert.AreEqual(1, TestClassA.NullableUIntField);
            TestClassA.NullableUIntField = 4000000000;
            Assert.AreEqual(2000000000, f(1));
            Assert.AreEqual(2000000000, TestClassA.NullableUIntField);
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