using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PowerAssign
{
    [TestFixture]
    public class TestStaticMember
    {
        [Test]
        public void TestDoubleField()
        {
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<double, double>> exp = Expression.Lambda<Func<double, double>>(Expression.PowerAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetField("DoubleField")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            TestClassA.DoubleField = 0;
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(1, TestClassA.DoubleField);
            TestClassA.DoubleField = 1;
            Assert.AreEqual(1, f(2));
            Assert.AreEqual(1, TestClassA.DoubleField);
            TestClassA.DoubleField = 2;
            Assert.AreEqual(16, f(4));
            Assert.AreEqual(16, TestClassA.DoubleField);
            TestClassA.DoubleField = -1;
            Assert.AreEqual(1, f(2));
            Assert.AreEqual(1, TestClassA.DoubleField);
        }

        [Test]
        public void TestNullableDouble()
        {
            ParameterExpression b = Expression.Parameter(typeof(double?), "b");
            Expression<Func<double?, double?>> exp = Expression.Lambda<Func<double?, double?>>(Expression.PowerAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("NullableDoubleProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            TestClassA.NullableDoubleProp = 0;
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(1, TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = 1;
            Assert.AreEqual(1, f(2));
            Assert.AreEqual(1, TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = 2;
            Assert.AreEqual(16, f(4));
            Assert.AreEqual(16, TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = -1;
            Assert.AreEqual(1, f(2));
            Assert.AreEqual(1, TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = null;
            Assert.IsNull(f(2));
            Assert.IsNull(TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = 1;
            Assert.IsNull(f(null));
            Assert.IsNull(TestClassA.NullableDoubleProp);
            Assert.IsNull(f(null));
            Assert.IsNull(TestClassA.NullableDoubleProp);
        }

        public class TestClassA
        {
            public static double? NullableDoubleProp { get; set; }
            public static double DoubleField;
        }
    }
}