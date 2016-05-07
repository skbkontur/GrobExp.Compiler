using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestListInit
    {
        [Test]
        public void TestList()
        {
            Expression<Func<List<int>>> exp = () => new List<int> {1, 2};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var list = f();
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
        }

        [Test]
        public void TestDictionary()
        {
            Expression<Func<Dictionary<int, string>>> exp = () => new Dictionary<int, string> {{1, "zzz"}, {2, "qxx"}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var dict = f();
            Assert.IsNotNull(dict);
            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("zzz", dict[1]);
            Assert.AreEqual("qxx", dict[2]);
        }

        [Test]
        public void TestStruct()
        {
            Expression<Func<TestStructA>> exp = () => new TestStructA(3) {1, 2};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var list = f();
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
        }

        public struct TestStructA : IEnumerable<int>
        {
            public TestStructA(int x)
            {
                list = new List<int>();
            }

            public IEnumerator<int> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(int x)
            {
                list.Add(x);
            }

            public int Count { get { return list.Count; } }

            public int this[int index] { get { return list[index]; } set { list[index] = value; } }
            private readonly List<int> list;
        }
    }
}