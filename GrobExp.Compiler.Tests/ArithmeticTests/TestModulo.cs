using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.ArithmeticTests
{
    public class TestModulo : TestBase
    {
        [Test]
        public void Test1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a % b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 3));
            ClassicAssert.AreEqual(-1, f(-3, 2));
        }

        [Test]
        public void Test2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a % b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 3));
            ClassicAssert.AreEqual(-1, f(-3, 2));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void Test3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a % b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 3));
            ClassicAssert.AreEqual(-1, f(-3, 2));
            ClassicAssert.AreEqual(2000000000, f(2000000000, 20000000000));
            ClassicAssert.IsNull(f(null, 2));
            ClassicAssert.IsNull(f(1, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void Test4()
        {
            Expression<Func<uint, uint, uint>> exp = (a, b) => a % b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 3));
            ClassicAssert.AreEqual(1, f(uint.MaxValue - 3 + 1, 2));
        }

        [Test]
        public void Test5()
        {
            Expression<Func<int?, int, int?>> exp = (a, b) => a % b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(1, f(1, 2));
            ClassicAssert.AreEqual(2, f(5, 3));
            ClassicAssert.AreEqual(-1, f(-3, 2));
            ClassicAssert.IsNull(f(null, 2));
        }
    }
}