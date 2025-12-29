using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.ModuloAssign
{
    [TestFixture]
    public class TestInstanceMember
    {
        [Test]
        public void TestIntProp()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.ModuloAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntProp = 1};
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.IntProp);
            o.IntProp = 5;
            ClassicAssert.AreEqual(2, f(o, 3));
            ClassicAssert.AreEqual(2, o.IntProp);
            o.IntProp = -3;
            ClassicAssert.AreEqual(-1, f(o, 2));
            ClassicAssert.AreEqual(-1, o.IntProp);
            ClassicAssert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntProp = 1};
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.IntProp);
            o.IntProp = 5;
            ClassicAssert.AreEqual(2, f(o, 3));
            ClassicAssert.AreEqual(2, o.IntProp);
            o.IntProp = -3;
            ClassicAssert.AreEqual(-1, f(o, 2));
            ClassicAssert.AreEqual(-1, o.IntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.ModuloAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntProp = 1};
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableIntProp);
            o.NullableIntProp = 5;
            ClassicAssert.AreEqual(2, f(o, 3));
            ClassicAssert.AreEqual(2, o.NullableIntProp);
            o.NullableIntProp = -3;
            ClassicAssert.AreEqual(-1, f(o, 2));
            ClassicAssert.AreEqual(-1, o.NullableIntProp);
            ClassicAssert.IsNull(f(null, 1));
            o.NullableIntProp = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableIntProp);
            o.NullableIntProp = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntProp);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntProp);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntProp = 1};
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableIntProp);
            o.NullableIntProp = 5;
            ClassicAssert.AreEqual(2, f(o, 3));
            ClassicAssert.AreEqual(2, o.NullableIntProp);
            o.NullableIntProp = -3;
            ClassicAssert.AreEqual(-1, f(o, 2));
            ClassicAssert.AreEqual(-1, o.NullableIntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntProp = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableIntProp);
            o.NullableIntProp = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntProp);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableIntProp);
        }

        [Test]
        public void TestUInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<TestClassA, uint, uint>> exp = Expression.Lambda<Func<TestClassA, uint, uint>>(Expression.ModuloAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("UIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntField = 1};
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.UIntField);
            o.UIntField = 5;
            ClassicAssert.AreEqual(2, f(o, 3));
            ClassicAssert.AreEqual(2, o.UIntField);
            o.UIntField = uint.MaxValue - 3 + 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.UIntField);
            ClassicAssert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntField = 1};
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.UIntField);
            o.UIntField = 5;
            ClassicAssert.AreEqual(2, f(o, 3));
            ClassicAssert.AreEqual(2, o.UIntField);
            o.UIntField = uint.MaxValue - 3 + 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.UIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        public class TestClassA
        {
            public int IntProp { get; set; }
            public int? NullableIntProp { get; set; }
            public uint UIntField;
        }
    }
}