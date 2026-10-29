using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests
{
    [TestFixture]
    public class TestNewObjectCreation
    {
        [Test]
        public void TestNew()
        {
            Expression<Func<int, string>> exp = length => new string('z', length);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual("zzz", f(3));
        }

        [Test]
        public void TestMemberInit1()
        {
            Expression<Func<int, string, TestClassA>> exp = (i, s) => new TestClassA {S = s, Y = i, B = new TestClassB {S = "qxx"}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = f(10, "zzz");
            ClassicAssert.AreEqual(10, a.Y);
            ClassicAssert.AreEqual("zzz", a.S);
            ClassicAssert.IsNotNull(a.B);
            ClassicAssert.AreEqual("qxx", a.B.S);
        }

        [Test]
        public void TestMemberInit2()
        {
            Expression<Func<int?, string, TestStructA>> exp = (i, s) => new TestStructA {S = s, X = i, B = new TestStructB {S = "qxx"}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = f(10, "zzz");
            ClassicAssert.AreEqual(10, a.X);
            ClassicAssert.AreEqual("zzz", a.S);
            ClassicAssert.AreEqual("qxx", a.B.S);
        }

        [Test]
        public void TestMemberInit3()
        {
            Expression<Func<int, int, TestClassA>> exp = (i, j) => new TestClassA {IntArray = new[] {i, j}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = f(10, 20);
            ClassicAssert.IsNotNull(a.IntArray);
            ClassicAssert.AreEqual(2, a.IntArray.Length);
            ClassicAssert.AreEqual(10, a.IntArray[0]);
            ClassicAssert.AreEqual(20, a.IntArray[1]);
        }

        [Test]
        public void TestMemberInit4()
        {
            Expression<Func<int, int, TestClassA>> exp = (i, j) => new TestClassA {IntList = new List<int> {i, j}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = f(10, 20);
            ClassicAssert.IsNotNull(a.IntList);
            ClassicAssert.AreEqual(2, a.IntList.Count);
            ClassicAssert.AreEqual(10, a.IntList[0]);
            ClassicAssert.AreEqual(20, a.IntList[1]);
        }

        [Test]
        public void TestNewArrayInit1()
        {
            Expression<Func<int?, int?, int?, int?[]>> exp = (a, b, c) => new[] {a, b, c};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(1, null, 2);
            ClassicAssert.IsNotNull(arr);
            ClassicAssert.AreEqual(3, arr.Length);
            ClassicAssert.AreEqual(1, arr[0]);
            ClassicAssert.AreEqual(null, arr[1]);
            ClassicAssert.AreEqual(2, arr[2]);
        }

        [Test]
        public void TestNewArrayInit2()
        {
            Expression<Func<string, string, string, string[]>> exp = (a, b, c) => new[] {a, b, c};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f("zzz", null, "qxx");
            ClassicAssert.IsNotNull(arr);
            ClassicAssert.AreEqual(3, arr.Length);
            ClassicAssert.AreEqual("zzz", arr[0]);
            ClassicAssert.AreEqual(null, arr[1]);
            ClassicAssert.AreEqual("qxx", arr[2]);
        }

        [Test]
        public void TestNewArrayInit3()
        {
            Expression<Func<long, long, long, long[]>> exp = (a, b, c) => new[] {a, b, c};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(1, long.MaxValue, long.MinValue);
            ClassicAssert.IsNotNull(arr);
            ClassicAssert.AreEqual(3, arr.Length);
            ClassicAssert.AreEqual(1, arr[0]);
            ClassicAssert.AreEqual(long.MaxValue, arr[1]);
            ClassicAssert.AreEqual(long.MinValue, arr[2]);
        }

        [Test]
        public void TestNewArrayBounds1()
        {
            Expression<Func<int, int?[]>> exp = i => new int?[i];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(1);
            ClassicAssert.IsNotNull(arr);
            ClassicAssert.AreEqual(1, arr.Length);
            ClassicAssert.AreEqual(null, arr[0]);
        }

        [Test]
        public void TestNewArrayBounds2()
        {
            Expression<Func<TestClassA, int?[]>> exp = a => new int?[a.Y];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(null);
            ClassicAssert.IsNotNull(arr);
            ClassicAssert.AreEqual(0, arr.Length);
            arr = f(new TestClassA {Y = 1});
            ClassicAssert.IsNotNull(arr);
            ClassicAssert.AreEqual(1, arr.Length);
            ClassicAssert.AreEqual(null, arr[0]);
        }

        [Test]
        public void TestNewArrayBounds3()
        {
            Expression<Func<int, int, int?[,]>> exp = (i, j) => new int?[i, j];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var arr = f(1, 2);
            ClassicAssert.IsNotNull(arr);
            ClassicAssert.AreEqual(2, arr.Length);
            ClassicAssert.AreEqual(null, arr[0, 0]);
            ClassicAssert.AreEqual(null, arr[0, 1]);
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