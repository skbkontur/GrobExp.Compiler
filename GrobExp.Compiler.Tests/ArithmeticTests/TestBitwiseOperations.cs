using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.ArithmeticTests
{
    public class TestBitwiseOperations : TestBase
    {
        [Test]
        public void TestAnd1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a & b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 123));
            ClassicAssert.AreEqual(1, f(3, 5));
            ClassicAssert.AreEqual(17235476 & 73172563, f(17235476, 73172563));
        }

        [Test]
        public void TestAnd2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a & b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 123));
            ClassicAssert.AreEqual(1, f(3, 5));
            ClassicAssert.AreEqual(17235476 & 73172563, f(17235476, 73172563));
            ClassicAssert.IsNull(f(null, 1));
            ClassicAssert.IsNull(f(123, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void TestAnd3()
        {
            Expression<Func<int, int?, int?>> exp = (a, b) => a & b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 123));
            ClassicAssert.AreEqual(1, f(3, 5));
            ClassicAssert.AreEqual(17235476 & 73172563, f(17235476, 73172563));
            ClassicAssert.IsNull(f(123, null));
        }

        [Test]
        public void TestAnd4()
        {
            Expression<Func<long, long, long>> exp = (a, b) => a & b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, f(0, 123));
            ClassicAssert.AreEqual(1, f(3, 5));
            ClassicAssert.AreEqual(172354712312316 & 73123123172563, f(172354712312316, 73123123172563));
        }

        [Test]
        public void TestOr1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a | b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(123, f(0, 123));
            ClassicAssert.AreEqual(7, f(3, 5));
            ClassicAssert.AreEqual(17235476 | 73172563, f(17235476, 73172563));
        }

        [Test]
        public void TestOr2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a | b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(123, f(0, 123));
            ClassicAssert.AreEqual(7, f(3, 5));
            ClassicAssert.AreEqual(17235476 | 73172563, f(17235476, 73172563));
            ClassicAssert.IsNull(f(null, 1));
            ClassicAssert.IsNull(f(123, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void TestOr3()
        {
            Expression<Func<int, int?, int?>> exp = (a, b) => a | b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(123, f(0, 123));
            ClassicAssert.AreEqual(7, f(3, 5));
            ClassicAssert.AreEqual(17235476 | 73172563, f(17235476, 73172563));
            ClassicAssert.IsNull(f(123, null));
        }

        [Test]
        public void TestOr4()
        {
            Expression<Func<long, long, long>> exp = (a, b) => a | b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(123, f(0, 123));
            ClassicAssert.AreEqual(7, f(3, 5));
            ClassicAssert.AreEqual(172354712312316 | 73123123172563, f(172354712312316, 73123123172563));
        }

        [Test]
        public void TestXor1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a ^ b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(123, f(0, 123));
            ClassicAssert.AreEqual(6, f(3, 5));
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(17235476, 73172563));
        }

        [Test]
        public void TestXor2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a ^ b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(123, f(0, 123));
            ClassicAssert.AreEqual(6, f(3, 5));
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(17235476, 73172563));
            ClassicAssert.IsNull(f(null, 1));
            ClassicAssert.IsNull(f(123, null));
            ClassicAssert.IsNull(f(null, null));
        }

        [Test]
        public void TestXor3()
        {
            Expression<Func<int, int?, int?>> exp = (a, b) => a ^ b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(123, f(0, 123));
            ClassicAssert.AreEqual(6, f(3, 5));
            ClassicAssert.AreEqual(17235476 ^ 73172563, f(17235476, 73172563));
            ClassicAssert.IsNull(f(123, null));
        }

        [Test]
        public void TestXor4()
        {
            Expression<Func<long, long, long>> exp = (a, b) => a ^ b;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(123, f(0, 123));
            ClassicAssert.AreEqual(6, f(3, 5));
            ClassicAssert.AreEqual(172354712312316 ^ 73123123172563, f(172354712312316, 73123123172563));
        }

        [Test]
        public void TestNot1()
        {
            Expression<Func<int, int>> exp = a => ~a;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(~123456789, f(123456789));
        }

        [Test]
        public void TestNot2()
        {
            Expression<Func<int?, int?>> exp = a => ~a;
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(~123456789, f(123456789));
            ClassicAssert.IsNull(f(null));
        }

        [Test]
        public void TestOnesComplement1()
        {
            ParameterExpression a = Expression.Parameter(typeof(int), "a");
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.OnesComplement(a), a);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(~123456789, f(123456789));
        }

        [Test]
        public void TestOnesComplement2()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?), "a");
            Expression<Func<int?, int?>> exp = Expression.Lambda<Func<int?, int?>>(Expression.OnesComplement(a), a);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(-1, f(0));
            ClassicAssert.AreEqual(0, f(-1));
            ClassicAssert.AreEqual(~123456789, f(123456789));
            ClassicAssert.IsNull(f(null));
        }
    }
}