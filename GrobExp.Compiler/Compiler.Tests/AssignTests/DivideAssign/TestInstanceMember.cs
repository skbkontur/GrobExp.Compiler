using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.DivideAssign
{
    [TestFixture]
    public class TestInstanceMember
    {
        [Test]
        public void TestIntProp()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.DivideAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntProp = 1};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntProp);
            o.IntProp = -3;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.IntProp);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntProp = 1};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntProp);
            o.IntProp = -3;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.IntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestDoubleField()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<TestClassA, double, double>> exp = Expression.Lambda<Func<TestClassA, double, double>>(Expression.DivideAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleField = 1};
            Assert.AreEqual(0.5, f(o, 2));
            Assert.AreEqual(0.5, o.DoubleField);
            o.DoubleField = 5;
            Assert.AreEqual(2.5, f(o, 2));
            Assert.AreEqual(2.5, o.DoubleField);
            o.DoubleField = -3;
            Assert.AreEqual(-1.5, f(o, 2));
            Assert.AreEqual(-1.5, o.DoubleField);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleField = 1};
            Assert.AreEqual(0.5, f(o, 2));
            Assert.AreEqual(0.5, o.DoubleField);
            o.DoubleField = 5;
            Assert.AreEqual(2.5, f(o, 2));
            Assert.AreEqual(2.5, o.DoubleField);
            o.DoubleField = -3;
            Assert.AreEqual(-1.5, f(o, 2));
            Assert.AreEqual(-1.5, o.DoubleField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.DivideAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntProp = 1};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntProp);
            o.NullableIntProp = -3;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.NullableIntProp);
            Assert.IsNull(f(null, 1));
            o.NullableIntProp = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntProp);
            o.NullableIntProp = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntProp);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntProp);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntProp = 1};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntProp);
            o.NullableIntProp = -3;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.NullableIntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntProp = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntProp);
            o.NullableIntProp = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntProp);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntProp);
        }

        [Test]
        public void TestUInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<TestClassA, uint, uint>> exp = Expression.Lambda<Func<TestClassA, uint, uint>>(Expression.DivideAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("UIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntField = 1};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.UIntField);
            o.UIntField = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.UIntField);
            o.UIntField = uint.MaxValue - 3 + 1;
            Assert.AreEqual(2147483646, f(o, 2));
            Assert.AreEqual(2147483646, o.UIntField);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntField = 1};
            Assert.AreEqual(0, f(o, 2));
            Assert.AreEqual(0, o.UIntField);
            o.UIntField = 5;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.UIntField);
            o.UIntField = uint.MaxValue - 3 + 1;
            Assert.AreEqual(2147483646, f(o, 2));
            Assert.AreEqual(2147483646, o.UIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        public class TestClassA
        {
            public int IntProp { get; set; }
            public int? NullableIntProp { get; set; }
            public double DoubleField;
            public uint UIntField;
        }
    }
}