using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.AssignTests.LeftShiftAssign
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
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(-2468, f(-1234, 1));

            exp = Expression.Lambda<Func<int, int, int>>(Expression.Block(typeof(int), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(-2468, f(-1234, 1));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(-2468, f(-1234, 1));
            ClassicAssert.IsNull(f(null, 1));
            ClassicAssert.IsNull(f(123, null));
            ClassicAssert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.Block(typeof(int?), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(-2468, f(-1234, 1));
            ClassicAssert.IsNull(f(null, 1));
            ClassicAssert.IsNull(f(123, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void Test3()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<int?, int, int?>> exp = Expression.Lambda<Func<int?, int, int?>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(-2468, f(-1234, 1));
            ClassicAssert.IsNull(f(null, 1));

            exp = Expression.Lambda<Func<int?, int, int?>>(Expression.Block(typeof(int?), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(-2468, f(-1234, 1));
            ClassicAssert.IsNull(f(null, 1));
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<uint, int, uint>> exp = Expression.Lambda<Func<uint, int, uint>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(2147483648, f(1, 31));

            exp = Expression.Lambda<Func<uint, int, uint>>(Expression.Block(typeof(uint), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(2147483648, f(1, 31));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<uint?, int, uint?>> exp = Expression.Lambda<Func<uint?, int, uint?>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(2147483648, f(1, 31));
            ClassicAssert.IsNull(f(null, 1));

            exp = Expression.Lambda<Func<uint?, int, uint?>>(Expression.Block(typeof(uint?), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(2147483648, f(1, 31));
            ClassicAssert.IsNull(f(null, 1));
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<uint?, int?, uint?>> exp = Expression.Lambda<Func<uint?, int?, uint?>>(Expression.LeftShiftAssign(a, b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(2147483648, f(1, 31));
            ClassicAssert.IsNull(f(null, 1));
            ClassicAssert.IsNull(f(123, null));
            ClassicAssert.IsNull(f(null, null));

            exp = Expression.Lambda<Func<uint?, int?, uint?>>(Expression.Block(typeof(uint?), Expression.LeftShiftAssign(a, b), a), a, b);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 0));
            ClassicAssert.AreEqual(0, f(0, 10));
            ClassicAssert.AreEqual(1024, f(1, 10));
            ClassicAssert.AreEqual(2468, f(1234, 1));
            ClassicAssert.AreEqual(16, f(1, 100));
            ClassicAssert.AreEqual(2147483648, f(1, 31));
            ClassicAssert.IsNull(f(null, 1));
            ClassicAssert.IsNull(f(123, null));
            ClassicAssert.IsNull(f(null, null));
        }
    }
}