using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.ModuloAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.ModuloAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(-1, f(-3, 2));

            exp = Expression.Lambda<Func<int, int, int>>(Expression.Block(typeof(int), Expression.ModuloAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(-1, f(-3, 2));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.ModuloAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.Block(typeof(int?), Expression.ModuloAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestUInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<uint, uint, uint>> exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.ModuloAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(1, f(uint.MaxValue - 3 + 1, 2));

            exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.Block(typeof(uint), Expression.ModuloAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(1, f(uint.MaxValue - 3 + 1, 2));
        }
    }
}