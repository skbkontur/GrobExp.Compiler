using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.PowerAssign
{
    [TestFixture]
    public class TestSimpleArray
    {
        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<TestClassA, double, double>> exp = Expression.Lambda<Func<TestClassA, double, double>>(Expression.PowerAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleArray")), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleArray = new[] {0.0}};
            ClassicAssert.AreEqual(1, f(o, 0));
            ClassicAssert.AreEqual(1, o.DoubleArray[0]);
            o.DoubleArray[0] = 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.DoubleArray[0]);
            o.DoubleArray[0] = 2;
            ClassicAssert.AreEqual(16, f(o, 4));
            ClassicAssert.AreEqual(16, o.DoubleArray[0]);
            o.DoubleArray[0] = -1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.DoubleArray[0]);
            ClassicAssert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleArray = new[] {0.0}};
            ClassicAssert.AreEqual(1, f(o, 0));
            ClassicAssert.AreEqual(1, o.DoubleArray[0]);
            o.DoubleArray[0] = 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.DoubleArray[0]);
            o.DoubleArray[0] = 2;
            ClassicAssert.AreEqual(16, f(o, 4));
            ClassicAssert.AreEqual(16, o.DoubleArray[0]);
            o.DoubleArray[0] = -1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.DoubleArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double?), "b");
            Expression<Func<TestClassA, double?, double?>> exp = Expression.Lambda<Func<TestClassA, double?, double?>>(Expression.PowerAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableDoubleArray")), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableDoubleArray = new double?[] {0.0}};
            ClassicAssert.AreEqual(1, f(o, 0));
            ClassicAssert.AreEqual(1, o.NullableDoubleArray[0]);
            o.NullableDoubleArray[0] = 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableDoubleArray[0]);
            o.NullableDoubleArray[0] = 2;
            ClassicAssert.AreEqual(16, f(o, 4));
            ClassicAssert.AreEqual(16, o.NullableDoubleArray[0]);
            o.NullableDoubleArray[0] = -1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableDoubleArray[0]);
            ClassicAssert.IsNull(f(null, 1));
            o.NullableDoubleArray[0] = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableDoubleArray[0]);
            o.NullableDoubleArray[0] = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableDoubleArray[0]);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableDoubleArray[0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableDoubleArray = new double?[] {1}};
            ClassicAssert.AreEqual(1, f(o, 0));
            ClassicAssert.AreEqual(1, o.NullableDoubleArray[0]);
            o.NullableDoubleArray[0] = 1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableDoubleArray[0]);
            o.NullableDoubleArray[0] = 2;
            ClassicAssert.AreEqual(16, f(o, 4));
            ClassicAssert.AreEqual(16, o.NullableDoubleArray[0]);
            o.NullableDoubleArray[0] = -1;
            ClassicAssert.AreEqual(1, f(o, 2));
            ClassicAssert.AreEqual(1, o.NullableDoubleArray[0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableDoubleArray[0] = null;
            ClassicAssert.IsNull(f(o, 2));
            ClassicAssert.IsNull(o.NullableDoubleArray[0]);
            o.NullableDoubleArray[0] = 1;
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableDoubleArray[0]);
            ClassicAssert.IsNull(f(o, null));
            ClassicAssert.IsNull(o.NullableDoubleArray[0]);
        }

        public class TestClassA
        {
            public double?[] NullableDoubleArray { get; set; }
            public double[] DoubleArray;
        }
    }
}