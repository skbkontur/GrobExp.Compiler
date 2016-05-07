using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.LeftShiftAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(-2468, f(-1234, 1));

            exp = Expression.Lambda<Func<int, int, int>>(Expression.Block(typeof(int), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(-2468, f(-1234, 1));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(-2468, f(-1234, 1));
            Assert.IsNull(f(null, 1));
            Assert.IsNull(f(123, null));
            Assert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.Block(typeof(int?), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(-2468, f(-1234, 1));
            Assert.IsNull(f(null, 1));
            Assert.IsNull(f(123, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void Test3()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int?, int, int?>> exp = Expression.Lambda<Func<int?, int, int?>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(-2468, f(-1234, 1));
            Assert.IsNull(f(null, 1));

            exp = Expression.Lambda<Func<int?, int, int?>>(Expression.Block(typeof(int?), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(-2468, f(-1234, 1));
            Assert.IsNull(f(null, 1));
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<uint, int, uint>> exp = Expression.Lambda<Func<uint, int, uint>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(2147483648, f(1, 31));

            exp = Expression.Lambda<Func<uint, int, uint>>(Expression.Block(typeof(uint), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(2147483648, f(1, 31));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<uint?, int, uint?>> exp = Expression.Lambda<Func<uint?, int, uint?>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(2147483648, f(1, 31));
            Assert.IsNull(f(null, 1));

            exp = Expression.Lambda<Func<uint?, int, uint?>>(Expression.Block(typeof(uint?), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(2147483648, f(1, 31));
            Assert.IsNull(f(null, 1));
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<uint?, int?, uint?>> exp = Expression.Lambda<Func<uint?, int?, uint?>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(2147483648, f(1, 31));
            Assert.IsNull(f(null, 1));
            Assert.IsNull(f(123, null));
            Assert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<uint?, int?, uint?>>(Expression.Block(typeof(uint?), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(2147483648, f(1, 31));
            Assert.IsNull(f(null, 1));
            Assert.IsNull(f(123, null));
            Assert.IsNull(f(null, null));
        }
    }
}