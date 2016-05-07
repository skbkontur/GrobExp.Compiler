using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.MultiplyAssign
{
    [TestFixture]
    public class TestInstanceMember
    {
        [Test]
        public void TestProp()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.MultiplyAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA();
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntProp);
            o.IntProp = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntProp);
            o.IntProp = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntProp);
            o.IntProp = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.IntProp);
            }
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA();
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntProp);
            o.IntProp = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntProp);
            o.IntProp = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntProp);
            o.IntProp = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.IntProp);
            }
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestField()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.MultiplyAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("IntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA();
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntField);
            o.IntField = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntField);
            o.IntField = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntField);
            o.IntField = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntField);
            o.IntField = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.IntField);
            }
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA();
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntField);
            o.IntField = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntField);
            o.IntField = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntField);
            o.IntField = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntField);
            o.IntField = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.IntField);
            }
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.MultiplyAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntProp = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntProp);
            o.NullableIntProp = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.NullableIntProp);
            o.NullableIntProp = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.NullableIntProp);
            o.NullableIntProp = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.NullableIntProp);
            }
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
            o = new TestClassA {NullableIntProp = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntProp);
            o.NullableIntProp = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.NullableIntProp);
            o.NullableIntProp = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.NullableIntProp);
            o.NullableIntProp = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(o, 2000000000));
                Assert.AreEqual(2000000000 * 2000000000, o.NullableIntProp);
            }
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
        public void TestCheckedSigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.MultiplyAssignChecked(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA();
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntProp);
            o.IntProp = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntProp);
            o.IntProp = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntProp);
            o.IntProp = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA();
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.IntProp);
            o.IntProp = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.IntProp);
            o.IntProp = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.IntProp);
            o.IntProp = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestCheckedNullableSigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.MultiplyAssignChecked(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntProp = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntProp);
            o.NullableIntProp = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.NullableIntProp);
            o.NullableIntProp = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.NullableIntProp);
            o.NullableIntProp = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
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
            o = new TestClassA {NullableIntProp = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntProp);
            o.NullableIntProp = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableIntProp);
            o.NullableIntProp = -2;
            Assert.AreEqual(6, f(o, -3));
            Assert.AreEqual(6, o.NullableIntProp);
            o.NullableIntProp = -2;
            Assert.AreEqual(-20, f(o, 10));
            Assert.AreEqual(-20, o.NullableIntProp);
            o.NullableIntProp = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 2000000000));
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
        public void TestCheckedUnsigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<TestClassA, uint, uint>> exp = Expression.Lambda<Func<TestClassA, uint, uint>>(Expression.MultiplyAssignChecked(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("UIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA();
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntField);
            o.UIntField = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.UIntField);
            o.UIntField = 2000000000;
            Assert.AreEqual(4000000000, f(o, 2));
            Assert.AreEqual(4000000000, o.UIntField);
            o.UIntField = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 3));
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA();
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntField);
            o.UIntField = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.UIntField);
            o.UIntField = 2000000000;
            Assert.AreEqual(4000000000, f(o, 2));
            Assert.AreEqual(4000000000, o.UIntField);
            o.UIntField = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 3));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestCheckedNullableUnsigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint?), "b");
            Expression<Func<TestClassA, uint?, uint?>> exp = Expression.Lambda<Func<TestClassA, uint?, uint?>>(Expression.MultiplyAssignChecked(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableUIntField);
            o.NullableUIntField = 2000000000;
            Assert.AreEqual(4000000000, f(o, 2));
            Assert.AreEqual(4000000000, o.NullableUIntField);
            o.NullableUIntField = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 3));
            Assert.IsNull(f(null, 1));
            o.NullableUIntField = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntField);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntField);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableUIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(2, f(o, 2));
            Assert.AreEqual(2, o.NullableUIntField);
            o.NullableUIntField = 2000000000;
            Assert.AreEqual(4000000000, f(o, 2));
            Assert.AreEqual(4000000000, o.NullableUIntField);
            o.NullableUIntField = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, 3));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableUIntField = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntField);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntField);
        }

        public class TestClassA
        {
            public int IntProp { get; set; }
            public int? NullableIntProp { get; set; }
            public int IntField;
            public uint UIntField;
            public uint? NullableUIntField;
        }
    }
}