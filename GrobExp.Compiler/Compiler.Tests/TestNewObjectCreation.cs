using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestNewObjectCreation
    {
        [Test]
        public void TestNew()
        {
            Expression<Func<int, string>> exp = length => new string('z', length);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(3));
        }

        [Test]
        public void TestMemberInit1()
        {
            Expression<Func<int, string, TestClassA>> exp = (i, s) => new TestClassA {S = s, Y = i, B = new TestClassB {S = "qxx"}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = f(10, "zzz");
            Assert.AreEqual(10, a.Y);
            Assert.AreEqual("zzz", a.S);
            Assert.IsNotNull(a.B);
            Assert.AreEqual("qxx", a.B.S);
        }

        [Test]
        public void TestMemberInit2()
        {
            Expression<Func<int?, string, TestStructA>> exp = (i, s) => new TestStructA {S = s, X = i, B = new TestStructB {S = "qxx"}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = f(10, "zzz");
            Assert.AreEqual(10, a.X);
            Assert.AreEqual("zzz", a.S);
            Assert.AreEqual("qxx", a.B.S);
        }

        [Test]
        public void TestMemberInit3()
        {
            Expression<Func<int, int, TestClassA>> exp = (i, j) => new TestClassA {IntArray = new[] {i, j}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = f(10, 20);
            Assert.IsNotNull(a.IntArray);
            Assert.AreEqual(2, a.IntArray.Length);
            Assert.AreEqual(10, a.IntArray[0]);
            Assert.AreEqual(20, a.IntArray[1]);
        }

        [Test]
        public void TestMemberInit4()
        {
            Expression<Func<int, int, TestClassA>> exp = (i, j) => new TestClassA {IntList = new List<int> {i, j}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = f(10, 20);
            Assert.IsNotNull(a.IntList);
            Assert.AreEqual(2, a.IntList.Count);
            Assert.AreEqual(10, a.IntList[0]);
            Assert.AreEqual(20, a.IntList[1]);
        }

        [Test]
        public void TestNewArrayInit1()
        {
            Expression<Func<int?, int?, int?, int?[]>> exp = (a, b, c) => new[] {a, b, c};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(1, null, 2);
            Assert.IsNotNull(arr);
            Assert.AreEqual(3, arr.Length);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(null, arr[1]);
            Assert.AreEqual(2, arr[2]);
        }

        [Test]
        public void TestNewArrayInit2()
        {
            Expression<Func<string, string, string, string[]>> exp = (a, b, c) => new[] {a, b, c};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f("zzz", null, "qxx");
            Assert.IsNotNull(arr);
            Assert.AreEqual(3, arr.Length);
            Assert.AreEqual("zzz", arr[0]);
            Assert.AreEqual(null, arr[1]);
            Assert.AreEqual("qxx", arr[2]);
        }

        [Test]
        public void TestNewArrayInit3()
        {
            Expression<Func<long, long, long, long[]>> exp = (a, b, c) => new[] {a, b, c};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(1, long.MaxValue, long.MinValue);
            Assert.IsNotNull(arr);
            Assert.AreEqual(3, arr.Length);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(long.MaxValue, arr[1]);
            Assert.AreEqual(long.MinValue, arr[2]);
        }

        [Test]
        public void TestNewArrayBounds1()
        {
            Expression<Func<int, int?[]>> exp = i => new int?[i];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(1);
            Assert.IsNotNull(arr);
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(null, arr[0]);
        }

        [Test]
        public void TestNewArrayBounds2()
        {
            Expression<Func<TestClassA, int?[]>> exp = a => new int?[a.Y];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(null);
            Assert.IsNotNull(arr);
            Assert.AreEqual(0, arr.Length);
            arr = f(new TestClassA {Y = 1});
            Assert.IsNotNull(arr);
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(null, arr[0]);
        }

        [Test]
        public void TestNewArrayBounds3()
        {
            Expression<Func<int, int, int?[,]>> exp = (i, j) => new int?[i, j];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(1, 2);
            Assert.IsNotNull(arr);
            Assert.AreEqual(2, arr.Length);
            Assert.AreEqual(null, arr[0, 0]);
            Assert.AreEqual(null, arr[0, 1]);
        }

        public struct TestStructA
        {
            public string S { get; set; }
            public TestStructB B { get; set; }
            public int? X { get; set; }
            public int Y { get; set; }
        }

        public struct TestStructB
        {
            public string S { get; set; }
        }

        public class TestClassA
        {
            public string S { get; set; }
            public TestClassB B { get; set; }
            public int[] IntArray { get; set; }
            public List<int> IntList { get; set; }
            public int Y;
        }

        public class TestClassB
        {
            public string S { get; set; }
        }
    }
}