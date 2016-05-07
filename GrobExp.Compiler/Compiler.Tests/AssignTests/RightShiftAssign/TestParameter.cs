using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.RightShiftAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.RightShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(-2, f(-3, 1));

            exp = Expression.Lambda<Func<int, int, int>>(Expression.Block(typeof(int), Expression.RightShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(-2, f(-3, 1));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.RightShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(-2, f(-3, 1));
            Assert.IsNull(f(null, 1));
            Assert.IsNull(f(123, null));
            Assert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.Block(typeof(int?), Expression.RightShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(-2, f(-3, 1));
            Assert.IsNull(f(null, 1));
            Assert.IsNull(f(123, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void Test3()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int?, int, int?>> exp = Expression.Lambda<Func<int?, int, int?>>(Expression.RightShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(-2, f(-3, 1));
            Assert.IsNull(f(null, 1));

            exp = Expression.Lambda<Func<int?, int, int?>>(Expression.Block(typeof(int?), Expression.RightShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(-2, f(-3, 1));
            Assert.IsNull(f(null, 1));
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<uint, int, uint>> exp = Expression.Lambda<Func<uint, int, uint>>(Expression.RightShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(2000000000, f(4000000000, 1));

            exp = Expression.Lambda<Func<uint, int, uint>>(Expression.Block(typeof(uint), Expression.RightShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(2000000000, f(4000000000, 1));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<uint?, int, uint?>> exp = Expression.Lambda<Func<uint?, int, uint?>>(Expression.RightShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(2000000000, f(4000000000, 1));
            Assert.IsNull(f(null, 1));

            exp = Expression.Lambda<Func<uint?, int, uint?>>(Expression.Block(typeof(uint?), Expression.RightShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(2000000000, f(4000000000, 1));
            Assert.IsNull(f(null, 1));
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<uint?, int?, uint?>> exp = Expression.Lambda<Func<uint?, int?, uint?>>(Expression.RightShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(2000000000, f(4000000000, 1));
            Assert.IsNull(f(null, 1));
            Assert.IsNull(f(123, null));
            Assert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<uint?, int?, uint?>>(Expression.Block(typeof(uint?), Expression.RightShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(2000000000, f(4000000000, 1));
            Assert.IsNull(f(null, 1));
            Assert.IsNull(f(123, null));
            Assert.IsNull(f(null, null));
        }
    }
}