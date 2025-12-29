using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests
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
            ClassicAssert.IsNotNull(list);
            ClassicAssert.AreEqual(2, list.Count);
            ClassicAssert.AreEqual(1, list[0]);
            ClassicAssert.AreEqual(2, list[1]);
        }

        [Test]
        public void TestDictionary()
        {
            Expression<Func<Dictionary<int, string>>> exp = () => new Dictionary<int, string> {{1, "zzz"}, {2, "qxx"}};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var dict = f();
            ClassicAssert.IsNotNull(dict);
            ClassicAssert.AreEqual(2, dict.Count);
            ClassicAssert.AreEqual("zzz", dict[1]);
            ClassicAssert.AreEqual("qxx", dict[2]);
        }

        [Test]
        public void TestStruct()
        {
            Expression<Func<TestStructA>> exp = () => new TestStructA(3) {1, 2};
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var list = f();
            ClassicAssert.IsNotNull(list);
            ClassicAssert.AreEqual(2, list.Count);
            ClassicAssert.AreEqual(1, list[0]);
            ClassicAssert.AreEqual(2, list[1]);
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