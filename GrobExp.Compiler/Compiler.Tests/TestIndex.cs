using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestIndex
    {
        [Test]
        public void TestMultidimensionalArray1()
        {
            Expression<Func<TestClassA, string>> exp = o => o.StringArray[1, 2];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA {StringArray = new string[2,3]};
            a.StringArray[1, 2] = "zzz";
            Assert.AreEqual("zzz", f(a));
        }

        [Test]
        public void TestMultidimensionalArray2()
        {
            Expression<Func<TestClassA, string[,]>> path = o => o.StringArray;
            Expression<Func<TestClassA, string>> exp = Expression.Lambda<Func<TestClassA, string>>(Expression.ArrayAccess(path.Body, Expression.Constant(1), Expression.Constant(2)), path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA {StringArray = new string[2,3]};
            a.StringArray[1, 2] = "zzz";
            Assert.AreEqual("zzz", f(a));
        }

        [Test]
        public void TestMultidimensionalArray3()
        {
            Expression<Func<TestClassA, bool>> exp = o => o.BoolArray[1, 2];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA {BoolArray = new bool[2,3]};
            a.BoolArray[1, 2] = true;
            Assert.AreEqual(true, f(a));
        }

        [Test]
        public void TestMultidimensionalArray4()
        {
            Expression<Func<TestClassA, bool[,]>> path = o => o.BoolArray;
            Expression<Func<TestClassA, bool>> exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.ArrayAccess(path.Body, Expression.Constant(1), Expression.Constant(2)), path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA {BoolArray = new bool[2,3]};
            a.BoolArray[1, 2] = true;
            Assert.AreEqual(true, f(a));
        }

        [Test]
        public void TestComplexIndexer1()
        {
            Expression<Func<TestClassA, string>> exp = o => o["zzz", 1];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA();
            a["zzz", 1] = "qxx";
            Assert.AreEqual("qxx", f(a));
        }

        [Test]
        public void TestComplexIndexer2()
        {
            Expression<Func<TestClassA, string>> exp = o => o.B["zzz", 1];
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA {B = new TestClassB()};
            a.B["zzz", 1] = "qxx";
            Assert.AreEqual("qxx", f(a));
        }

        [Test]
        public void TestComplexIndexer3()
        {
            var parameter = Expression.Parameter(typeof(TestClassA));
            var exp = Expression.Lambda<Func<TestClassA, string>>(Expression.MakeIndex(parameter, typeof(TestClassA).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA();
            a["zzz", 1] = "qxx";
            Assert.AreEqual("qxx", f(a));
        }

        [Test]
        public void TestSimpleIndex1()
        {
            Expression<Func<TestClassA, string>> exp = o => o.IntArray[0].ToString();
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(null, f(null));
            Assert.AreEqual(null, f(new TestClassA()));
            Assert.AreEqual(null, f(new TestClassA {IntArray = new int[0]}));
            Assert.AreEqual("123", f(new TestClassA {IntArray = new[] {123}}));
        }

        [Test]
        public void TestSimpleIndex2()
        {
            Expression<Func<TestClassA, int[]>> path = o => o.IntArray;
            ParameterExpression variable = Expression.Parameter(typeof(int[]));
            var body = Expression.Block(typeof(string), new[] {variable}, Expression.Assign(variable, path.Body), Expression.Call(Expression.ArrayIndex(variable, Expression.Constant(0)), typeof(object).GetMethod("ToString")));
            var exp = Expression.Lambda<Func<TestClassA, string>>(body, path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(null, f(null));
            Assert.AreEqual(null, f(new TestClassA()));
            Assert.AreEqual(null, f(new TestClassA {IntArray = new int[0]}));
            Assert.AreEqual("123", f(new TestClassA {IntArray = new[] {123}}));
        }

        [Test]
        public void TestSimpleIndex3()
        {
            Expression<Func<TestClassA, int>> path = o => o.IntArray[0];
            ParameterExpression variable = Expression.Parameter(typeof(int));
            var body = Expression.Block(typeof(string), new[] {variable}, Expression.Assign(variable, path.Body), Expression.Call(variable, typeof(object).GetMethod("ToString")));
            var exp = Expression.Lambda<Func<TestClassA, string>>(body, path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("0", f(null));
            Assert.AreEqual("0", f(new TestClassA()));
            Assert.AreEqual("0", f(new TestClassA {IntArray = new int[0]}));
            Assert.AreEqual("123", f(new TestClassA {IntArray = new[] {123}}));
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
            public string[,] StringArray { get; set; }
            public bool[,] BoolArray { get; set; }

            public string this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    string[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new string[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new string[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            public int? X;
            public Guid Guid = Guid.Empty;
            public Guid? NullableGuid;
            public bool? NullableBool;
            public int Y;
            public bool Bool;

            private readonly Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
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

            public string this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    string[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new string[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new string[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            public int? X;
            public int Y;

            private readonly Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
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

            public string S;
        }

        public class TestClassE
        {
            public string S { get; set; }
            public int X { get; set; }
        }

        public struct TestStructA
        {
            public string S { get; set; }
            public TestStructB[] ArrayB { get; set; }
            public int? X { get; set; }
            public int Y { get; set; }
        }

        public struct TestStructB
        {
            public string S { get; set; }
            public int Y { get; set; }
        }
    }
}