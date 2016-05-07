using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PowerAssign
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
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.DoubleField);
            o.DoubleField = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleField);
            o.DoubleField = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.DoubleField);
            o.DoubleField = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleField);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleField = 0};
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.DoubleField);
            o.DoubleField = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleField);
            o.DoubleField = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.DoubleField);
            o.DoubleField = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleField);
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
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.NullableDoubleProp);
            o.NullableDoubleProp = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleProp);
            o.NullableDoubleProp = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.NullableDoubleProp);
            o.NullableDoubleProp = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleProp);
            Assert.IsNull(f(null, 1));
            o.NullableDoubleProp = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableDoubleProp);
            o.NullableDoubleProp = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleProp);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleProp);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableDoubleProp = 0};
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.NullableDoubleProp);
            o.NullableDoubleProp = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleProp);
            o.NullableDoubleProp = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.NullableDoubleProp);
            o.NullableDoubleProp = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleProp);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableDoubleProp = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableDoubleProp);
            o.NullableDoubleProp = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleProp);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleProp);
        }

        public class TestClassA
        {
            public double? NullableDoubleProp { get; set; }
            public double DoubleField;
        }
    }
}