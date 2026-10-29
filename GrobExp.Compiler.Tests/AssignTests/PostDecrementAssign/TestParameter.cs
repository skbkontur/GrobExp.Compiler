using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.PostDecrementAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.PostDecrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0));
            ClassicAssert.AreEqual(1, f(1));
            ClassicAssert.AreEqual(int.MinValue, f(int.MinValue));

            exp = Expression.Lambda<Func<int, int>>(Expression.Block(typeof(int), Expression.PostDecrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(0));
            ClassicAssert.AreEqual(0, f(1));
            ClassicAssert.AreEqual(int.MaxValue, f(int.MinValue));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.PostDecrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0));
            ClassicAssert.AreEqual(1, f(1));
            ClassicAssert.AreEqual(int.MinValue, f(int.MinValue));
            ClassicAssert.IsNull(f(null));

            exp = Expression.Lambda<Func<int?, int?>>(Expression.Block(typeof(int?), Expression.PostDecrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(0));
            ClassicAssert.AreEqual(0, f(1));
            ClassicAssert.AreEqual(int.MaxValue, f(int.MinValue));
            ClassicAssert.IsNull(f(null));
        }

        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double), "a");
            Expression<Func<double, double>> exp = Expression.Lambda<Func<double, double>>(Expression.PostDecrementAssign(a), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0));
            ClassicAssert.AreEqual(1, f(1));
            ClassicAssert.AreEqual(0.5, f(0.5));

            exp = Expression.Lambda<Func<double, double>>(Expression.Block(typeof(double), Expression.PostDecrementAssign(a), a), a);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(0));
            ClassicAssert.AreEqual(0, f(1));
            ClassicAssert.AreEqual(-0.5, f(0.5));
        }
    }
}