using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.TryCatchTests
{
    public class TestFault : TestBase
    {
        [Test]
        public void Test()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(TestClassA), "b");
            TryExpression tryExpr =
                Expression.TryFault(
                    Expression.Call(
                        Expression.MultiplyChecked(
                            Expression.Convert(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("X")), typeof(int)),
                            Expression.Convert(Expression.MakeMemberAccess(b, typeof(TestClassA).GetProperty("X")), typeof(int))
                            ), typeof(object).GetMethod("ToString")),
                    Expression.Assign(Expression.MakeMemberAccess(null, typeof(TestFault).GetField("B")), Expression.Constant(true))
                    );
            var exp = Expression.Lambda<Func<TestClassA, TestClassA, string>>(tryExpr, a, b);
            var f = CompileToMethod(exp, CompilerOptions.None);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(null, null));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<OverflowException>(() => f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            Assert.AreEqual("1000000", f(new TestClassA {X = 1000}, new TestClassA {X = 1000}));
            Assert.IsFalse(B);
            B = false;

            f = CompileToMethod(exp, CompilerOptions.All);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(null, null));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            Assert.Throws<OverflowException>(() => f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            Assert.AreEqual("1000000", f(new TestClassA {X = 1000}, new TestClassA {X = 1000}));
            Assert.IsFalse(B);
            B = false;
        }

        public static bool B;
        public static string F;

        public class TestClassA
        {
            public object X { get; set; }
            public bool B { get; set; }
            public string Message { get; set; }
        }
    }
}