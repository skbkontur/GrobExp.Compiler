using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.RightShiftAssign
{
    [TestFixture]
    public class TestInstanceMember
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntProp = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.IntProp);
            o.IntProp = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.IntProp);
            o.IntProp = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.IntProp);
            o.IntProp = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.IntProp);
            o.IntProp = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.IntProp);
            o.IntProp = -3;
            ClassicAssert.AreEqual(-2, f(o, 1));
            ClassicAssert.AreEqual(-2, o.IntProp);
            ClassicAssert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntProp = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.IntProp);
            o.IntProp = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.IntProp);
            o.IntProp = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.IntProp);
            o.IntProp = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.IntProp);
            o.IntProp = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.IntProp);
            o.IntProp = -3;
            ClassicAssert.AreEqual(-2, f(o, 1));
            ClassicAssert.AreEqual(-2, o.IntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntField = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.NullableIntField);
            o.NullableIntField = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.NullableIntField);
            o.NullableIntField = -3;
            ClassicAssert.AreEqual(-2, f(o, 1));
            ClassicAssert.AreEqual(-2, o.NullableIntField);
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
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.NullableIntField);
            o.NullableIntField = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.NullableIntField);
            o.NullableIntField = -3;
            ClassicAssert.AreEqual(-2, f(o, 1));
            ClassicAssert.AreEqual(-2, o.NullableIntField);
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

        [Test]
        public void Test3()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int?>> exp = Expression.Lambda<Func<TestClassA, int, int?>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntField = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.NullableIntField);
            o.NullableIntField = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.NullableIntField);
            o.NullableIntField = -3;
            ClassicAssert.AreEqual(-2, f(o, 1));
            ClassicAssert.AreEqual(-2, o.NullableIntField);
            ClassicAssert.IsNull(f(null, 1));
            o.NullableIntField = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableIntField);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntField = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.NullableIntField);
            o.NullableIntField = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableIntField);
            o.NullableIntField = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.NullableIntField);
            o.NullableIntField = -3;
            ClassicAssert.AreEqual(-2, f(o, 1));
            ClassicAssert.AreEqual(-2, o.NullableIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntField = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableIntField);
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, uint>> exp = Expression.Lambda<Func<TestClassA, int, uint>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("UIntProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntProp = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.UIntProp);
            o.UIntProp = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.UIntProp);
            o.UIntProp = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.UIntProp);
            o.UIntProp = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.UIntProp);
            o.UIntProp = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.UIntProp);
            o.UIntProp = 4000000000;
            ClassicAssert.AreEqual(2000000000, f(o, 1));
            ClassicAssert.AreEqual(2000000000, o.UIntProp);
            ClassicAssert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntProp = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.UIntProp);
            o.UIntProp = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.UIntProp);
            o.UIntProp = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.UIntProp);
            o.UIntProp = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.UIntProp);
            o.UIntProp = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.UIntProp);
            o.UIntProp = 4000000000;
            ClassicAssert.AreEqual(2000000000, f(o, 1));
            ClassicAssert.AreEqual(2000000000, o.UIntProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, uint?>> exp = Expression.Lambda<Func<TestClassA, int, uint?>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntField = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.NullableUIntField);
            o.NullableUIntField = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.NullableUIntField);
            o.NullableUIntField = 4000000000;
            ClassicAssert.AreEqual(2000000000, f(o, 1));
            ClassicAssert.AreEqual(2000000000, o.NullableUIntField);
            ClassicAssert.IsNull(f(null, 1));
            o.NullableUIntField = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableUIntField);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableUIntField = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.NullableUIntField);
            o.NullableUIntField = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.NullableUIntField);
            o.NullableUIntField = 4000000000;
            ClassicAssert.AreEqual(2000000000, f(o, 1));
            ClassicAssert.AreEqual(2000000000, o.NullableUIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableUIntField = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableUIntField);
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, uint?>> exp = Expression.Lambda<Func<TestClassA, int?, uint?>>(Expression.RightShiftAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntField = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.NullableUIntField);
            o.NullableUIntField = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.NullableUIntField);
            o.NullableUIntField = 4000000000;
            ClassicAssert.AreEqual(2000000000, f(o, 1));
            ClassicAssert.AreEqual(2000000000, o.NullableUIntField);
            ClassicAssert.IsNull(f(null, 1));
            o.NullableUIntField = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableUIntField);
            o.NullableUIntField = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableUIntField);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableUIntField);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableUIntField = 0};
            ClassicAssert.AreEqual(0, f(o, 0));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 0;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 1024;
            ClassicAssert.AreEqual(1, f(o, 10));
            ClassicAssert.AreEqual(1, o.NullableUIntField);
            o.NullableUIntField = 1023;
            ClassicAssert.AreEqual(0, f(o, 10));
            ClassicAssert.AreEqual(0, o.NullableUIntField);
            o.NullableUIntField = 3;
            ClassicAssert.AreEqual(1, f(o, 1));
            ClassicAssert.AreEqual(1, o.NullableUIntField);
            o.NullableUIntField = 4000000000;
            ClassicAssert.AreEqual(2000000000, f(o, 1));
            ClassicAssert.AreEqual(2000000000, o.NullableUIntField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableUIntField = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableUIntField);
            o.NullableUIntField = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableUIntField);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableUIntField);
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