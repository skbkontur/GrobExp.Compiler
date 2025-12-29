using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.PreIncrementAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.PreIncrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(int.MinValue, f(int.MaxValue));

            exp = Expression.Lambda<Func<int, int>>(Expression.Block(typeof(int), Expression.PreIncrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(int.MinValue, f(int.MaxValue));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.PreIncrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(int.MinValue, f(int.MaxValue));
            ClassicAssert.IsNull(f(null));

            exp = Expression.Lambda<Func<int?, int?>>(Expression.Block(typeof(int?), Expression.PreIncrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(int.MinValue, f(int.MaxValue));
            ClassicAssert.IsNull(f(null));
        }

        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double), "a");
            Expression<Func<double, double>> exp = Expression.Lambda<Func<double, double>>(Expression.PreIncrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(0.5, f(-0.5));

            exp = Expression.Lambda<Func<double, double>>(Expression.Block(typeof(double), Expression.PreIncrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(0.5, f(-0.5));
        }
    }
}