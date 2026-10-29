using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.ArithmeticTests
{
    public class TestDivide : TestBase
    {
        [Test]
        public void Test1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 2));
            ClassicAssert.AreEqual(-1, f(-3, 2));
        }

        [Test]
        public void Test2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 2));
            ClassicAssert.AreEqual(-1, f(-3, 2));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void Test3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 2));
            ClassicAssert.AreEqual(-1, f(-3, 2));
            ClassicAssert.AreEqual(0, f(2000000000, 20000000000));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void Test4()
        {
            Expression<Func<double, double, double>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0.5, f(1, 2));
            ClassicAssert.AreEqual(2.5, f(5, 2));
            ClassicAssert.AreEqual(-1.5, f(-3, 2));
        }

        [Test]
        public void Test5()
        {
            Expression<Func<uint, uint, uint>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 2));
            ClassicAssert.AreEqual(2147483646, f(uint.MaxValue - 3 + 1, 2));
        }

        [Test]
        public void Test6()
        {
            Expression<Func<int?, int, int?>> exp = (a, b) => a / b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 2));
            ClassicAssert.AreEqual(-1, f(-3, 2));
            ClassicAssert.IsNull(f(null, 2));
        }
    }
}