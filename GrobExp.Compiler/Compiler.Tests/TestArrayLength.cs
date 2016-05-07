using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestArrayLength
    {
        [Test]
        public void Test()
        {
            Expression<Func<TestClassA, int>> exp = a => a.ArrayB.Length;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(null));
            Assert.AreEqual(0, f(new TestClassA()));
            Assert.AreEqual(1, f(new TestClassA {ArrayB = new TestClassB[1]}));
        }

        public class TestClassA
        {
            public string S { get; set; }
            public int Y { get; set; }
            public TestClassB[] ArrayB { get; set; }
        }

        public class TestClassB
        {
            public string S { get; set; }
        }
    }
}