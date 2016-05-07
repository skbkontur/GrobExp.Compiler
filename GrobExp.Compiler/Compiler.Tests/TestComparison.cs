using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    public class TestComparison: TestBase
    {
        [Test]
        public void TestGreaterThan1()
        {
            Expression<Func<int, int, bool>> exp = (a, b) => a > b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
        }

        [Test]
        public void TestGreaterThan2()
        {
            Expression<Func<int?, int?, bool>> exp = (a, b) => a > b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestGreaterThan3()
        {
            Expression<Func<long?, int?, bool>> exp = (a, b) => a > b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestGreaterThan4()
        {
            Expression<Func<uint, uint, bool>> exp = (a, b) => a > b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(1, uint.MaxValue));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(1, uint.MaxValue));
        }

        [Test]
        public void TestGreaterThan5()
        {
            Expression<Func<int?, int?, bool>> exp = (a, b) => !(a > b);
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsTrue(f(1, null));
            Assert.IsTrue(f(null, 1));
            Assert.IsTrue(f(null, null));
        }

        [Test]
        public void TestGreaterThanOrEqual1()
        {
            Expression<Func<int, int, bool>> exp = (a, b) => a >= b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
        }

        [Test]
        public void TestGreaterThanOrEqual2()
        {
            Expression<Func<int?, int?, bool>> exp = (a, b) => a >= b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestGreaterThanOrEqual3()
        {
            Expression<Func<long?, int?, bool>> exp = (a, b) => a >= b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestGreaterThanOrEqual4()
        {
            Expression<Func<uint, uint, bool>> exp = (a, b) => a >= b;
            var f = Compile(exp,CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(1, uint.MaxValue));
            Assert.IsTrue(f(3000000000, 3000000000));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(1, uint.MaxValue));
            Assert.IsTrue(f(3000000000, 3000000000));
        }

        [Test]
        public void TestGreaterThanOrEqual5()
        {
            Expression<Func<int?, int?, bool>> exp = (a, b) => !(a >= b);
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsFalse(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsFalse(f(-1, -1));
            Assert.IsTrue(f(1, null));
            Assert.IsTrue(f(null, 1));
            Assert.IsTrue(f(null, null));
        }

        [Test]
        public void TestLessThan1()
        {
            Expression<Func<int, int, bool>> exp = (a, b) => a < b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
        }

        [Test]
        public void TestLessThan2()
        {
            Expression<Func<int?, int?, bool>> exp = (a, b) => a < b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestLessThan3()
        {
            Expression<Func<long?, int?, bool>> exp = (a, b) => a < b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestLessThan4()
        {
            Expression<Func<uint, uint, bool>> exp = (a, b) => a < b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(1, uint.MaxValue));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(1, uint.MaxValue));
        }

        [Test]
        public void TestLessThan5()
        {
            Expression<Func<int?, int?, bool>> exp = (a, b) => !(a < b);
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsTrue(f(1, null));
            Assert.IsTrue(f(null, 1));
            Assert.IsTrue(f(null, null));
        }

        [Test]
        public void TestLessThanOrEqual1()
        {
            Expression<Func<int, int, bool>> exp = (a, b) => a <= b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
        }

        [Test]
        public void TestLessThanOrEqual2()
        {
            Expression<Func<int?, int?, bool>> exp = (a, b) => a <= b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestLessThanOrEqual3()
        {
            Expression<Func<long?, int?, bool>> exp = (a, b) => a <= b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestLessThanOrEqual4()
        {
            Expression<Func<uint, uint, bool>> exp = (a, b) => a <= b;
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(1, uint.MaxValue));
            Assert.IsTrue(f(3000000000, 3000000000));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(1, uint.MaxValue));
            Assert.IsTrue(f(3000000000, 3000000000));
        }

        [Test]
        public void TestLessThanOrEqual5()
        {
            Expression<Func<int?, int?, bool>> exp = (a, b) => !(a <= b);
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsFalse(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
            f = Compile(exp, CompilerOptions.None);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsFalse(f(-1, -1));
            Assert.IsTrue(f(1, null));
            Assert.IsTrue(f(null, 1));
            Assert.IsTrue(f(null, null));
        }
    }
}