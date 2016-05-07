using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.DivideAssign
{
    [TestFixture]
    public class TestStaticMember
    {
        [Test]
        public void TestIntProp()
        {
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.DivideAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("IntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.IntProp = 1;
            Assert.AreEqual(0, f(2));
            Assert.AreEqual(0, TestClassA.IntProp);
            TestClassA.IntProp = 5;
            Assert.AreEqual(2, f(2));
            Assert.AreEqual(2, TestClassA.IntProp);
            TestClassA.IntProp = -3;
            Assert.AreEqual(-1, f(2));
            Assert.AreEqual(-1, TestClassA.IntProp);
        }

        [Test]
        public void TestDoubleField()
        {
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<double, double>> exp = Expression.Lambda<Func<double, double>>(Expression.DivideAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("DoubleField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.DoubleField = 1;
            Assert.AreEqual(0.5, f(2));
            Assert.AreEqual(0.5, TestClassA.DoubleField);
            TestClassA.DoubleField = 5;
            Assert.AreEqual(2.5, f(2));
            Assert.AreEqual(2.5, TestClassA.DoubleField);
            TestClassA.DoubleField = -3;
            Assert.AreEqual(-1.5, f(2));
            Assert.AreEqual(-1.5, TestClassA.DoubleField);
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.DivideAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("NullableIntProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            TestClassA.NullableIntProp = 1;
            Assert.AreEqual(0, f(2));
            Assert.AreEqual(0, TestClassA.NullableIntProp);
            TestClassA.NullableIntProp = 5;
            Assert.AreEqual(2, f(2));
            Assert.AreEqual(2, TestClassA.NullableIntProp);
            TestClassA.NullableIntProp = -3;
            Assert.AreEqual(-1, f(2));
            Assert.AreEqual(-1, TestClassA.NullableIntProp);
            TestClassA.NullableIntProp = null;
            Assert.IsNull(f(2));
            Assert.IsNull(TestClassA.NullableIntProp);
            TestClassA.NullableIntProp = 1;
            Assert.IsNull(f(null));
            Assert.IsNull(TestClassA.NullableIntProp);
            Assert.IsNull(f(null));
            Assert.IsNull(TestClassA.NullableIntProp);
        }

        [Test]
        public void TestUInt()
        {
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<uint, uint>> exp = Expression.Lambda<Func<uint, uint>>(Expression.DivideAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("UIntField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.UIntField = 1;
            Assert.AreEqual(0, f(2));
            Assert.AreEqual(0, TestClassA.UIntField);
            TestClassA.UIntField = 5;
            Assert.AreEqual(2, f(2));
            Assert.AreEqual(2, TestClassA.UIntField);
            TestClassA.UIntField = uint.MaxValue - 3 + 1;
            Assert.AreEqual(2147483646, f(2));
            Assert.AreEqual(2147483646, TestClassA.UIntField);
        }

        public class TestClassA
        {
            public static int IntProp { get; set; }
            public static int? NullableIntProp { get; set; }
            public static double DoubleField;
            public static uint UIntField;
        }
    }
}