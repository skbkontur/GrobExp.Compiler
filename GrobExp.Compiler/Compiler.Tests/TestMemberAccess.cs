using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestMemberAccess
    {
        [Test]
        public void TestPropertyAccess()
        {
            Expression<Func<TestClassA, string>> exp = o => o.S;
            Func<TestClassA, string> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA {S = "zzz"}), Is.EqualTo("zzz"));
        }

        [Test]
        public void TestFieldAccess()
        {
            Expression<Func<TestClassA, int>> exp = o => o.Y;
            Func<TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(0));
            Assert.That(compiledExp(new TestClassA {Y = 1}), Is.EqualTo(1));
        }

        public class TestClassA
        {
            public string S { get; set; }
            public int Y { get; set; }
        }
    }
}