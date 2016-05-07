using System;
using System.Linq;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestCheckNullReference
    {
        [Test]
        public void Test2()
        {
            Expression<Func<TestClassA, string>> exp = o => o.B.S;
            Func<TestClassA, string> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA()), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA {B = new TestClassB {S = "zzz"}}), Is.EqualTo("zzz"));
        }

        [Test]
        public void Test3()
        {
            Expression<Func<TestClassA, int?>> exp = o => o.B.X;
            Func<TestClassA, int?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA()), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA {B = new TestClassB {X = 1}}), Is.EqualTo(1));
        }

        [Test]
        public void Test4()
        {
            Expression<Func<TestClassA, int?>> exp = o => o.B.F2( /*new Qzz{X = 1}*/1);
            Func<TestClassA, int?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA()), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA {B = new TestClassB {X = 1}}), Is.EqualTo(1));
        }

        [Test]
        public void Test6()
        {
            Expression<Func<TestClassA, int?>> exp = o => o.ArrayB.Sum(b => b.X);
            Func<TestClassA, int?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA()), Is.EqualTo(0));
            Assert.That(compiledExp(new TestClassA {ArrayB = new[] {new TestClassB()}}), Is.EqualTo(0));
            Assert.That(compiledExp(new TestClassA {ArrayB = new[] {new TestClassB {X = 2}}}), Is.EqualTo(2));
            Assert.That(compiledExp(new TestClassA {ArrayB = new[] {new TestClassB {X = 2}, null}}), Is.EqualTo(2));
        }

        [Test]
        public void Test7a()
        {
            Expression<Func<TestClassA, int>> exp = o => o.B.Y + o.Y;
            Func<TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(0));
            Assert.That(compiledExp(new TestClassA()), Is.EqualTo(0));
            Assert.That(compiledExp(new TestClassA {Y = 1}), Is.EqualTo(1));
            Assert.That(compiledExp(new TestClassA {Y = 1, B = new TestClassB()}), Is.EqualTo(1));
            Assert.That(compiledExp(new TestClassA {B = new TestClassB {Y = 2}}), Is.EqualTo(2));
            Assert.That(compiledExp(new TestClassA {Y = 1, B = new TestClassB {Y = 2}}), Is.EqualTo(3));
        }

        [Test]
        public void Test7b()
        {
            Expression<Func<TestClassA, int?>> exp = o => o.B.X + o.X;
            Func<TestClassA, int?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA()), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA {X = 1}), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA {X = 1, B = new TestClassB()}), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA {B = new TestClassB {X = 2}}), Is.EqualTo(null));
            Assert.That(compiledExp(new TestClassA {X = 1, B = new TestClassB {X = 2}}), Is.EqualTo(3));
        }

        [Test]
        public void Test8()
        {
            Expression<Func<TestClassA, int>> exp = o => o.ArrayB.Sum(b => b.Y);
            Func<TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(0));
            Assert.That(compiledExp(new TestClassA()), Is.EqualTo(0));
            Assert.That(compiledExp(new TestClassA {ArrayB = new[] {new TestClassB()}}), Is.EqualTo(0));
            Assert.That(compiledExp(new TestClassA {ArrayB = new[] {new TestClassB {Y = 2}}}), Is.EqualTo(2));
            Assert.That(compiledExp(new TestClassA {ArrayB = new[] {new TestClassB {Y = 2}, null}}), Is.EqualTo(2));
        }

        public class TestClassA
        {
            public int F(bool b)
            {
                return b ? 1 : 0;
            }

            public string S { get; set; }
            public TestClassA A { get; set; }
            public TestClassB B { get; set; }
            public TestClassB[] ArrayB { get; set; }
            public int[] IntArray { get; set; }
            public int? X;
            public Guid Guid = Guid.Empty;
            public Guid? NullableGuid;
            public bool? NullableBool;
            public int Y;
            public bool Bool;
        }

        public class TestClassB
        {
            public int? F2(int? x)
            {
                return x;
            }

            public int? F( /*Qzz*/ int a, int b)
            {
                return b;
            }

            public string S { get; set; }

            public TestClassC C { get; set; }
            public int? X;
            public int Y;
        }

        public class TestClassC
        {
            public string S { get; set; }

            public TestClassD D { get; set; }

            public TestClassD[] ArrayD { get; set; }
        }

        public class TestClassD
        {
            public TestClassE E { get; set; }
            public TestClassE[] ArrayE { get; set; }
            public string Z { get; set; }

            public int? X { get; set; }

            public readonly string S;
        }

        public class TestClassE
        {
            public string S { get; set; }
            public int X { get; set; }
        }
    }
}