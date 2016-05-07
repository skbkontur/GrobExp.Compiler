using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.ArithmeticTests
{
    public class TestDivide : TestBase
    {
        [Test]
        public void Test1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
        }

        [Test]
        public void Test2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void Test3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.AreEqual(0, f(2000000000, 20000000000));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void Test4()
        {
            Expression<Func<double, double, double>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0.5, f(1, 2));
            Assert.AreEqual(2.5, f(5, 2));
            Assert.AreEqual(-1.5, f(-3, 2));
        }

        [Test]
        public void Test5()
        {
            Expression<Func<uint, uint, uint>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(2147483646, f(uint.MaxValue - 3 + 1, 2));
        }

        [Test]
        public void Test6()
        {
            Expression<Func<int?, int, int?>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.IsNull(f(null, 2));
        }
    }
}