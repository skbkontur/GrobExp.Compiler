using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.PowerAssign
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
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(1, TestClassA.DoubleField);
            TestClassA.DoubleField = 1;
            ClassicAssert.AreEqual(1, f(2));
            ClassicAssert.AreEqual(1, TestClassA.DoubleField);
            TestClassA.DoubleField = 2;
            ClassicAssert.AreEqual(16, f(4));
            ClassicAssert.AreEqual(16, TestClassA.DoubleField);
            TestClassA.DoubleField = -1;
            ClassicAssert.AreEqual(1, f(2));
            ClassicAssert.AreEqual(1, TestClassA.DoubleField);
        }

        [Test]
        public void TestNullableDouble()
        {
            ParameterExpression b = Expression.Parameter(typeof(double?), "b");
            Expression<Func<double?, double?>> exp = Expression.Lambda<Func<double?, double?>>(Expression.PowerAssign(Expression.MakeMemberAccess(null, typeof(TestClassA).GetProperty("NullableDoubleProp")), b), b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            TestClassA.NullableDoubleProp = 0;
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(1, TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = 1;
            ClassicAssert.AreEqual(1, f(2));
            ClassicAssert.AreEqual(1, TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = 2;
            ClassicAssert.AreEqual(16, f(4));
            ClassicAssert.AreEqual(16, TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = -1;
            ClassicAssert.AreEqual(1, f(2));
            ClassicAssert.AreEqual(1, TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = null;
            ClassicAssert.IsNull(f(2));
            ClassicAssert.IsNull(TestClassA.NullableDoubleProp);
            TestClassA.NullableDoubleProp = 1;
            ClassicAssert.IsNull(f(null));
            ClassicAssert.IsNull(TestClassA.NullableDoubleProp);
            ClassicAssert.IsNull(f(null));
            ClassicAssert.IsNull(TestClassA.NullableDoubleProp);
        }

        public class TestClassA
        {
            public static double? NullableDoubleProp { get; set; }
            public static double DoubleField;
        }
    }
}