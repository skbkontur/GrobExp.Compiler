using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.ArithmeticTests
{
    public class TestShifts : TestBase
    {
        [Test]
        public void TestLeftShift1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a << b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(-2468, f(-1234, 1));
        }

        [Test]
        public void TestLeftShift2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a << b;
            var f = Compile(exp, CompilerOptions.All);
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
        public void TestLeftShift3()
        {
            Expression<Func<int?, int, int?>> exp = (a, b) => a << b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(-2468, f(-1234, 1));
            Assert.IsNull(f(null, 1));
        }

        [Test]
        public void TestLeftShift4()
        {
            Expression<Func<uint, int, uint>> exp = (a, b) => a << b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(2147483648, f(1, 31));
        }

        [Test]
        public void TestLeftShift5()
        {
            Expression<Func<uint?, int, uint?>> exp = (a, b) => a << b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1024, f(1, 10));
            Assert.AreEqual(2468, f(1234, 1));
            Assert.AreEqual(16, f(1, 100));
            Assert.AreEqual(2147483648, f(1, 31));
            Assert.IsNull(f(null, 1));
        }

        [Test]
        public void TestLeftShift6()
        {
            Expression<Func<uint?, int?, uint?>> exp = (a, b) => a << b;
            var f = Compile(exp, CompilerOptions.All);
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

        [Test]
        public void TestRightShift1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a >> b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(-2, f(-3, 1));
        }

        [Test]
        public void TestRightShift2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a >> b;
            var f = Compile(exp, CompilerOptions.All);
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
        public void TestRightShift3()
        {
            Expression<Func<uint, int, uint>> exp = (a, b) => a >> b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(2000000000, f(4000000000, 1));
        }

        [Test]
        public void TestRightShift4()
        {
            Expression<Func<uint?, int, uint?>> exp = (a, b) => a >> b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(0, f(0, 10));
            Assert.AreEqual(1, f(1024, 10));
            Assert.AreEqual(0, f(1023, 10));
            Assert.AreEqual(1, f(3, 1));
            Assert.AreEqual(2000000000, f(4000000000, 1));
            Assert.IsNull(f(null, 1));
        }

        [Test]
        public void TestRightShift5()
        {
            Expression<Func<uint?, int?, uint?>> exp = (a, b) => a >> b;
            var f = Compile(exp, CompilerOptions.All);
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