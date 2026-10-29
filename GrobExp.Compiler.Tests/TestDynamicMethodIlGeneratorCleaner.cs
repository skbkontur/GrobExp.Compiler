using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests
{
    public class TestDynamicMethodIlGeneratorCleaner : TestBase
    {
        [Test]
        public void TestILGeneratorIsClearedAfterCompilation()
        {
            Expression<Func<int, int>> exp = x => x * 2 + 1;
            var f = Compile(exp, CompilerOptions.All);
            var dynamicMethod = GetDynamicMethodOrIgnore(f);
            ClassicAssert.IsNull(ilGeneratorField.GetValue(dynamicMethod));
            ClassicAssert.AreEqual(43, f(21));
        }

        [Test]
        public void TestSubLambdasWorkAfterClearAndGc()
        {
            Expression<Func<TestClassA, bool>> exp = a => a.ArrayB.Any(b => b.S == a.S) && a.ArrayB.Select(b => b.S.Length).Sum() > 0;
            var f = Compile(exp, CompilerOptions.All);
            GetDynamicMethodOrIgnore(f);
            var subLambdas = new List<Delegate>();
            CollectDelegates(f.Target, subLambdas, new HashSet<object>());
            ClassicAssert.AreEqual(2, subLambdas.Count);
            foreach (var subLambda in subLambdas)
                ClassicAssert.IsNull(ilGeneratorField.GetValue(GetDynamicMethodOrIgnore(subLambda)));
            for (var i = 0; i < 3; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            ClassicAssert.IsTrue(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB {S = "zzz"}}}));
            ClassicAssert.IsFalse(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB {S = "qqq"}}}));
        }

        [Test]
        public void TestReflectionInvokeAndGetILGeneratorAfterClear()
        {
            Expression<Func<string, int>> exp = s => s.Length + 1;
            var f = Compile(exp, CompilerOptions.All);
            var dynamicMethod = GetDynamicMethodOrIgnore(f);
            // invoke the DynamicMethod itself: .NET Framework and .NET 6/7 forbid Invoke on the RTDynamicMethod wrapper from Delegate.Method
            var args = dynamicMethod.GetParameters().Length == 1 ? new object[] {"abc"} : new object[] {f.Target, "abc"};
            ClassicAssert.AreEqual(4, dynamicMethod.Invoke(null, args));
            ClassicAssert.IsNotNull(dynamicMethod.GetILGenerator()); // creates a fresh, unused generator; must not affect the compiled method
            ClassicAssert.AreEqual(4, f("abc"));
        }

        [Test]
        public void TestManyCompilationsInParallel()
        {
            var results = Enumerable.Range(0, 64).AsParallel().WithDegreeOfParallelism(8).Select(i =>
                {
                    var x = Expression.Parameter(typeof(int), "x");
                    var lambda = Expression.Lambda<Func<int, int>>(Expression.Add(Expression.Multiply(x, Expression.Constant(i)), Expression.Constant(1)), x);
                    return Compile(lambda, CompilerOptions.All)(2) == 2 * i + 1;
                }).ToArray();
            ClassicAssert.IsTrue(results.All(r => r));
        }

        // sub-lambda delegates are stored in the closure constants of the outer delegate
        private static void CollectDelegates(object obj, List<Delegate> result, HashSet<object> visited)
        {
            if (obj == null || obj is string || obj.GetType().IsPrimitive || !visited.Add(obj))
                return;
            if (obj is Delegate subLambda)
            {
                result.Add(subLambda);
                return;
            }
            if (obj is Array array)
            {
                foreach (var item in array)
                    CollectDelegates(item, result, visited);
                return;
            }
            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                CollectDelegates(field.GetValue(obj), result, visited);
        }

        private static DynamicMethod GetDynamicMethodOrIgnore(Delegate f)
        {
            if (ilGeneratorField == null)
                Assert.Ignore("DynamicMethod._ilGenerator is not available on this runtime");
            // .NET Framework and .NET 6/7 return RTDynamicMethod wrapper from Delegate.Method, the DynamicMethod itself is in its m_owner field
            var dynamicMethod = f.Method as DynamicMethod
                                ?? f.Method.GetType().GetField("m_owner", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(f.Method) as DynamicMethod;
            if (dynamicMethod == null)
                Assert.Ignore("Delegate.Method does not expose DynamicMethod on this runtime");
            return dynamicMethod;
        }

        private static readonly FieldInfo ilGeneratorField = typeof(DynamicMethod).GetField("_ilGenerator", BindingFlags.Instance | BindingFlags.NonPublic)
                                                             ?? typeof(DynamicMethod).GetField("m_ilGenerator", BindingFlags.Instance | BindingFlags.NonPublic);

        public class TestClassA
        {
            public string S { get; set; }
            public TestClassB[] ArrayB { get; set; }
        }

        public class TestClassB
        {
            public string S { get; set; }
        }
    }
}