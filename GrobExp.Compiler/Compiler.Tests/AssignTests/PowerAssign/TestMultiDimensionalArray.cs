using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PowerAssign
{
    [TestFixture]
    public class TestMultiDimensionalArray
    {
        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<TestClassA, double, double>> exp = Expression.Lambda<Func<TestClassA, double, double>>(Expression.PowerAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleArray = new double[1,1]};
            o.DoubleArray[0, 0] = 0;
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.DoubleArray[0, 0]);
            o.DoubleArray[0, 0] = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleArray[0, 0]);
            o.DoubleArray[0, 0] = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.DoubleArray[0, 0]);
            o.DoubleArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleArray[0, 0]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleArray = new double[1,1]};
            o.DoubleArray[0, 0] = 0;
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.DoubleArray[0, 0]);
            o.DoubleArray[0, 0] = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleArray[0, 0]);
            o.DoubleArray[0, 0] = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.DoubleArray[0, 0]);
            o.DoubleArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleArray[0, 0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullableDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double?), "b");
            Expression<Func<TestClassA, double?, double?>> exp = Expression.Lambda<Func<TestClassA, double?, double?>>(Expression.PowerAssign(Expression.ArrayAccess(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableDoubleArray")), Expression.Constant(0), Expression.Constant(0)), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableDoubleArray = new double?[1,1]};
            o.NullableDoubleArray[0, 0] = 0;
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.NullableDoubleArray[0, 0]);
            o.NullableDoubleArray[0, 0] = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleArray[0, 0]);
            o.NullableDoubleArray[0, 0] = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.NullableDoubleArray[0, 0]);
            o.NullableDoubleArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleArray[0, 0]);
            Assert.IsNull(f(null, 1));
            o.NullableDoubleArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableDoubleArray[0, 0]);
            o.NullableDoubleArray[0, 0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleArray[0, 0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleArray[0, 0]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableDoubleArray = new double?[1,1]};
            o.NullableDoubleArray[0, 0] = 0;
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.NullableDoubleArray[0, 0]);
            o.NullableDoubleArray[0, 0] = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleArray[0, 0]);
            o.NullableDoubleArray[0, 0] = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.NullableDoubleArray[0, 0]);
            o.NullableDoubleArray[0, 0] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleArray[0, 0]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableDoubleArray[0, 0] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableDoubleArray[0, 0]);
            o.NullableDoubleArray[0, 0] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleArray[0, 0]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleArray[0, 0]);
        }

        public class TestClassA
        {
            public double?[,] NullableDoubleArray { get; set; }
            public double[,] DoubleArray;
        }
    }
}