using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.TryCatchTests
{
    public class FilterExceptionTest : TestBase
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(TestClassA), "b");
            TryExpression tryExpr =
                Expression.TryCatchFinally(
                    Expression.Call(
                        Expression.MultiplyChecked(
                            Expression.Convert(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("X")), typeof(int)),
                            Expression.Convert(Expression.MakeMemberAccess(b, typeof(TestClassA).GetProperty("X")), typeof(int))
                            ), typeof(object).GetMethod("ToString")),
                    Expression.Assign(Expression.MakeMemberAccess(null, typeof(FilterExceptionTest).GetField("B")), Expression.Constant(true)),
                    Expression.Catch(
                        typeof(OverflowException),
                        Expression.Constant("Overflow"),
                        Expression.Equal(Expression.MakeMemberAccess(null, typeof(FilterExceptionTest).GetField("F")), Expression.Constant("zzz"))
                        ),
                    Expression.Catch(
                        typeof(InvalidCastException),
                        Expression.Constant("Invalid cast"),
                        Expression.Equal(Expression.MakeMemberAccess(null, typeof(FilterExceptionTest).GetField("F")), Expression.Constant("zzz"))
                        ),
                    Expression.Catch(
                        typeof(NullReferenceException),
                        Expression.Constant("Null reference"),
                        Expression.Equal(Expression.MakeMemberAccess(null, typeof(FilterExceptionTest).GetField("F")), Expression.Constant("zzz"))
                        )
                    );
            var exp = Expression.Lambda<Func<TestClassA, TestClassA, string>>(tryExpr, a, b);
            var f = CompileToMethod(exp, CompilerOptions.None);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(null, null));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(null, null));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Invalid cast", f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Invalid cast", f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Overflow", f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<OverflowException>(() => f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            Assert.AreEqual("1000000", f(new TestClassA {X = 1000}, new TestClassA {X = 1000}));
            Assert.IsTrue(B);
            B = false;

            f = CompileToMethod(exp, CompilerOptions.All);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(null, null));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(null, null));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Null reference", f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Invalid cast", f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Invalid cast", f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Overflow", f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<OverflowException>(() => f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            Assert.AreEqual("1000000", f(new TestClassA {X = 1000}, new TestClassA {X = 1000}));
            Assert.IsTrue(B);
            B = false;
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(TestClassA), "b");
            ParameterExpression overflow = Expression.Parameter(typeof(OverflowException), "overflow");
            ParameterExpression invalidCast = Expression.Parameter(typeof(InvalidCastException), "invalidCast");
            ParameterExpression nullReference = Expression.Parameter(typeof(NullReferenceException), "nullReference");
            TryExpression tryExpr =
                Expression.TryCatchFinally(
                    Expression.Call(
                        Expression.MultiplyChecked(
                            Expression.Convert(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("X")), typeof(int)),
                            Expression.Convert(Expression.MakeMemberAccess(b, typeof(TestClassA).GetProperty("X")), typeof(int))
                            ), typeof(object).GetMethod("ToString")),
                    Expression.Assign(Expression.MakeMemberAccess(null, typeof(FilterExceptionTest).GetField("B")), Expression.Constant(true)),
                    Expression.Catch(
                        overflow,
                        Expression.MakeMemberAccess(overflow, typeof(Exception).GetProperty("Message")),
                        Expression.Equal(Expression.MakeMemberAccess(null, typeof(FilterExceptionTest).GetField("F")), Expression.Constant("zzz"))
                        ),
                    Expression.Catch(
                        invalidCast,
                        Expression.MakeMemberAccess(invalidCast, typeof(Exception).GetProperty("Message")),
                        Expression.Equal(Expression.MakeMemberAccess(null, typeof(FilterExceptionTest).GetField("F")), Expression.Constant("zzz"))
                        ),
                    Expression.Catch(
                        nullReference,
                        Expression.MakeMemberAccess(nullReference, typeof(Exception).GetProperty("Message")),
                        Expression.Equal(Expression.MakeMemberAccess(null, typeof(FilterExceptionTest).GetField("F")), Expression.Constant("zzz"))
                        )
                    );
            var exp = Expression.Lambda<Func<TestClassA, TestClassA, string>>(Expression.Block(new[] {overflow, invalidCast, nullReference}, tryExpr), a, b);
            var f = CompileToMethod(exp, CompilerOptions.None);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(null, null));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(null, null));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Specified cast is not valid.", f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Specified cast is not valid.", f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Arithmetic operation resulted in an overflow.", f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<OverflowException>(() => f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            Assert.AreEqual("1000000", f(new TestClassA {X = 1000}, new TestClassA {X = 1000}));
            Assert.IsTrue(B);
            B = false;

            f = CompileToMethod(exp, CompilerOptions.All);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(null, null));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(null, null));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(null, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), null));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA {X = 1}, new TestClassA()));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Object reference not set to an instance of an object.", f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<NullReferenceException>(() => f(new TestClassA(), new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Specified cast is not valid.", f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = "zzz"}, new TestClassA {X = 1}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Specified cast is not valid.", f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<InvalidCastException>(() => f(new TestClassA {X = 1}, new TestClassA {X = "zzz"}));
            Assert.IsTrue(B);
            B = false;
            F = "zzz";
            Assert.AreEqual("Arithmetic operation resulted in an overflow.", f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            F = "qxx";
            Assert.Throws<OverflowException>(() => f(new TestClassA {X = 1000000}, new TestClassA {X = 1000000}));
            Assert.IsTrue(B);
            B = false;
            Assert.AreEqual("1000000", f(new TestClassA {X = 1000}, new TestClassA {X = 1000}));
            Assert.IsTrue(B);
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