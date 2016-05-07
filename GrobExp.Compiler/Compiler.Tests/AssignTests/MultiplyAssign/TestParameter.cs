using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.MultiplyAssign
{
    [TestFixture]
    public class TestParameter
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.MultiplyAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }

            exp = Expression.Lambda<Func<int, int, int>>(Expression.Block(typeof(int), Expression.MultiplyAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.MultiplyAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }

            exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.Block(typeof(int?), Expression.MultiplyAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }
        }

        [Test]
        public void Test3()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.MultiplyAssignChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));

            exp = Expression.Lambda<Func<int, int, int>>(Expression.Block(typeof(int), Expression.MultiplyAssignChecked(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.MultiplyAssignChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.AreEqual(-2000000000, f(200000000, -10));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));

            exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.Block(typeof(int?), Expression.MultiplyAssignChecked(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.AreEqual(-2000000000, f(200000000, -10));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<uint, uint, uint>> exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.MultiplyAssignChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(4000000000, f(2000000000, 2));
            Assert.Throws<OverflowException>(() => f(2000000000, 3));

            exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.Block(typeof(uint), Expression.MultiplyAssignChecked(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(4000000000, f(2000000000, 2));
            Assert.Throws<OverflowException>(() => f(2000000000, 3));
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint?), "b");
            Expression<Func<uint?, uint?, uint?>> exp = Expression.Lambda<Func<uint?, uint?, uint?>>(Expression.MultiplyAssignChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(4000000000, f(2000000000, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 3));

            exp = Expression.Lambda<Func<uint?, uint?, uint?>>(Expression.Block(typeof(uint?), Expression.MultiplyAssignChecked(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(4000000000, f(2000000000, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 3));
        }
    }
}