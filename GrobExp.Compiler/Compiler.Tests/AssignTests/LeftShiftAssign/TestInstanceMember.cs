using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.LeftShiftAssign
{
    [TestFixture]
    public class TestInstanceMember
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntProp = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.IntProp);
            o.IntProp = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.IntProp);
            o.IntProp = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.IntProp);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntProp = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.IntProp);
            o.IntProp = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.IntProp);
            o.IntProp = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.IntProp);
            o.IntProp = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.IntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableIntField);
            o.NullableIntField = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableIntField);
            o.NullableIntField = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableIntField);
            o.NullableIntField = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.NullableIntField);
            Assert.IsNull(f(null, 1));
            o.NullableIntField = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntField);
            o.NullableIntField = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntField);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntField);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableIntField);
            o.NullableIntField = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableIntField);
            o.NullableIntField = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableIntField);
            o.NullableIntField = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.NullableIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntField = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntField);
            o.NullableIntField = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntField);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntField);
        }

        [Test]
        public void Test3()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int?>> exp = Expression.Lambda<Func<TestClassA, int, int?>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableIntField);
            o.NullableIntField = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableIntField);
            o.NullableIntField = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableIntField);
            o.NullableIntField = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.NullableIntField);
            Assert.IsNull(f(null, 1));
            o.NullableIntField = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntField);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableIntField);
            o.NullableIntField = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableIntField);
            o.NullableIntField = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableIntField);
            o.NullableIntField = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.NullableIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntField = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntField);
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, uint>> exp = Expression.Lambda<Func<TestClassA, int, uint>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("UIntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntProp = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntProp);
            o.UIntProp = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.UIntProp);
            o.UIntProp = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.UIntProp);
            o.UIntProp = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.UIntProp);
            o.UIntProp = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.UIntProp);
            o.UIntProp = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.UIntProp);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntProp = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntProp);
            o.UIntProp = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.UIntProp);
            o.UIntProp = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.UIntProp);
            o.UIntProp = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.UIntProp);
            o.UIntProp = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.UIntProp);
            o.UIntProp = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.UIntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, uint?>> exp = Expression.Lambda<Func<TestClassA, int, uint?>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableUIntField);
            o.NullableUIntField = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.NullableUIntField);
            Assert.IsNull(f(null, 1));
            o.NullableUIntField = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntField);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableUIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableUIntField);
            o.NullableUIntField = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.NullableUIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableUIntField = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntField);
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, uint?>> exp = Expression.Lambda<Func<TestClassA, int?, uint?>>(Expression.LeftShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntField = 0};
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableUIntField);
            o.NullableUIntField = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.NullableUIntField);
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
            o.NullableUIntField = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableUIntField);
            o.NullableUIntField = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableUIntField);
            o.NullableUIntField = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.NullableUIntField);
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
            public uint UIntProp { get; set; }
            public int? NullableIntField;
            public uint? NullableUIntField;
        }
    }
}