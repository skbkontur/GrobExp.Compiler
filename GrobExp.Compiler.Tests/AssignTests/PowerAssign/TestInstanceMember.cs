using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.PowerAssign
{
    [TestFixture]
    public class TestInstanceMember
    {
        [Test]
        public void TestDoubleField()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<TestClassA, double, double>> exp = Expression.Lambda<Func<TestClassA, double, double>>(Expression.PowerAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleField")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleField = 0};
            ClassicAssert.AreEqual(1, f(o, 0));
            ClassicAssert.AreEqual(1, o.DoubleField);
            o.DoubleField = 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.DoubleField);
            o.DoubleField = 2;
            ClassicAssert.AreEqual(16, f(o, 4));
            ClassicAssert.AreEqual(16, o.DoubleField);
            o.DoubleField = -1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.DoubleField);
            ClassicAssert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleField = 0};
            ClassicAssert.AreEqual(1, f(o, 0));
            ClassicAssert.AreEqual(1, o.DoubleField);
            o.DoubleField = 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.DoubleField);
            o.DoubleField = 2;
            ClassicAssert.AreEqual(16, f(o, 4));
            ClassicAssert.AreEqual(16, o.DoubleField);
            o.DoubleField = -1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.DoubleField);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullableDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double?), "b");
            Expression<Func<TestClassA, double?, double?>> exp = Expression.Lambda<Func<TestClassA, double?, double?>>(Expression.PowerAssign(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableDoubleProp")), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableDoubleProp = 0};
            ClassicAssert.AreEqual(1, f(o, 0));
            ClassicAssert.AreEqual(1, o.NullableDoubleProp);
            o.NullableDoubleProp = 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableDoubleProp);
            o.NullableDoubleProp = 2;
            ClassicAssert.AreEqual(16, f(o, 4));
            ClassicAssert.AreEqual(16, o.NullableDoubleProp);
            o.NullableDoubleProp = -1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableDoubleProp);
            ClassicAssert.IsNull(f(null, 1));
            o.NullableDoubleProp = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableDoubleProp);
            o.NullableDoubleProp = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableDoubleProp);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableDoubleProp);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableDoubleProp = 0};
            ClassicAssert.AreEqual(1, f(o, 0));
            ClassicAssert.AreEqual(1, o.NullableDoubleProp);
            o.NullableDoubleProp = 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableDoubleProp);
            o.NullableDoubleProp = 2;
            ClassicAssert.AreEqual(16, f(o, 4));
            ClassicAssert.AreEqual(16, o.NullableDoubleProp);
            o.NullableDoubleProp = -1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableDoubleProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableDoubleProp = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableDoubleProp);
            o.NullableDoubleProp = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableDoubleProp);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableDoubleProp);
        }

        public class TestClassA
        {
            public double? NullableDoubleProp { get; set; }
            public double DoubleField;
        }
    }
}