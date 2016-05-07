using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestCustomOperators
    {
        [Test]
        public void TestAdd1()
        {
            Expression<Func<Fraction, Fraction, Fraction>> exp = (a, b) => a + b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(new Fraction(5, 6), f(new Fraction(1, 2), new Fraction(1, 3)));
            ParameterExpression parameterA = Expression.Parameter(typeof(Fraction));
            ParameterExpression parameterB = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction, Fraction>> exp2 = Expression.Lambda<Func<Fraction, Fraction, Fraction>>(Expression.AddChecked(parameterA, parameterB, ((BinaryExpression)exp.Body).Method), parameterA, parameterB);
            f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            Assert.AreEqual(new Fraction(5, 6), f(new Fraction(1, 2), new Fraction(1, 3)));
        }

        [Test]
        public void TestAdd2()
        {
            Expression<Func<decimal?, decimal?, decimal?>> exp = (a, b) => a + b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual(1, f(-1, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestSubtract1()
        {
            Expression<Func<Fraction, Fraction, Fraction>> exp = (a, b) => a - b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(new Fraction(1, 6), f(new Fraction(1, 2), new Fraction(1, 3)));
            ParameterExpression parameterA = Expression.Parameter(typeof(Fraction));
            ParameterExpression parameterB = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction, Fraction>> exp2 = Expression.Lambda<Func<Fraction, Fraction, Fraction>>(Expression.SubtractChecked(parameterA, parameterB, ((BinaryExpression)exp.Body).Method), parameterA, parameterB);
            f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            Assert.AreEqual(new Fraction(1, 6), f(new Fraction(1, 2), new Fraction(1, 3)));
        }

        [Test]
        public void TestSubtract2()
        {
            Expression<Func<decimal?, decimal?, decimal?>> exp = (a, b) => a - b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(-1, f(1, 2));
            Assert.AreEqual(1, f(-1, -2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestMultiply1()
        {
            Expression<Func<Fraction, Fraction, Fraction>> exp = (a, b) => a * b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(new Fraction(1, 6), f(new Fraction(1, 2), new Fraction(1, 3)));
            ParameterExpression parameterA = Expression.Parameter(typeof(Fraction));
            ParameterExpression parameterB = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction, Fraction>> exp2 = Expression.Lambda<Func<Fraction, Fraction, Fraction>>(Expression.MultiplyChecked(parameterA, parameterB, ((BinaryExpression)exp.Body).Method), parameterA, parameterB);
            f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            Assert.AreEqual(new Fraction(1, 6), f(new Fraction(1, 2), new Fraction(1, 3)));
        }

        [Test]
        public void TestMultiply2()
        {
            Expression<Func<decimal?, decimal?, decimal?>> exp = (a, b) => a * b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestDivide1()
        {
            Expression<Func<Fraction, Fraction, Fraction>> exp = (a, b) => a / b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(new Fraction(3, 2), f(new Fraction(1, 2), new Fraction(1, 3)));
        }

        [Test]
        public void TestDivide2()
        {
            Expression<Func<decimal?, decimal?, decimal?>> exp = (a, b) => a / b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0.5m, f(1, 2));
            Assert.AreEqual(2.5m, f(5, 2));
            Assert.AreEqual(-1.5m, f(-3, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestIncrement1()
        {
            var parameter = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction>> exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.Increment(parameter, typeof(Fraction).GetMethod("op_Increment")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(1, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(-1, 1)));
            exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.Block(typeof(Fraction), Expression.Increment(parameter, typeof(Fraction).GetMethod("op_Increment")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(-1, 1), f(new Fraction(-1, 1)));
        }

        [Test]
        public void TestIncrement2()
        {
            var parameter = Expression.Parameter(typeof(decimal?));
            Expression<Func<decimal?, decimal?>> exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.Increment(parameter, typeof(decimal).GetMethod("op_Increment")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(1m, f(0m));
            Assert.AreEqual(0m, f(-1m));
            exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.Block(typeof(decimal?), Expression.Increment(parameter, typeof(decimal).GetMethod("op_Increment")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(0m, f(0m));
            Assert.AreEqual(-1m, f(-1m));
        }

        [Test]
        public void TestDecrement1()
        {
            var parameter = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction>> exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.Decrement(parameter, typeof(Fraction).GetMethod("op_Decrement")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(-1, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(1, 1)));
            exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.Block(typeof(Fraction), Expression.Decrement(parameter, typeof(Fraction).GetMethod("op_Decrement")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(-1, 1), f(new Fraction(-1, 1)));
        }

        [Test]
        public void TestDecrement2()
        {
            var parameter = Expression.Parameter(typeof(decimal?));
            Expression<Func<decimal?, decimal?>> exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.Decrement(parameter, typeof(decimal).GetMethod("op_Decrement")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(-1m, f(0m));
            Assert.AreEqual(0m, f(1m));
            exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.Block(typeof(decimal?), Expression.Decrement(parameter, typeof(decimal).GetMethod("op_Decrement")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(0m, f(0m));
            Assert.AreEqual(-1m, f(-1m));
        }

        [Test]
        public void TestPreIncrementAssign1()
        {
            var parameter = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction>> exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.PreIncrementAssign(parameter, typeof(Fraction).GetMethod("op_Increment")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(1, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(-1, 1)));
            exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.Block(typeof(Fraction), Expression.PreIncrementAssign(parameter, typeof(Fraction).GetMethod("op_Increment")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(1, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(-1, 1)));
        }

        [Test]
        public void TestPreIncrementAssign2()
        {
            var parameter = Expression.Parameter(typeof(decimal?));
            Expression<Func<decimal?, decimal?>> exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.PreIncrementAssign(parameter, typeof(decimal).GetMethod("op_Increment")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(1m, f(0m));
            Assert.AreEqual(0m, f(-1m));
            exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.Block(typeof(decimal?), Expression.PreIncrementAssign(parameter, typeof(decimal).GetMethod("op_Increment")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(1m, f(0m));
            Assert.AreEqual(0m, f(-1m));
        }

        [Test]
        public void TestPostIncrementAssign1()
        {
            var parameter = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction>> exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.PostIncrementAssign(parameter, typeof(Fraction).GetMethod("op_Increment")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(-1, 1), f(new Fraction(-1, 1)));
            exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.Block(typeof(Fraction), Expression.PostIncrementAssign(parameter, typeof(Fraction).GetMethod("op_Increment")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(1, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(-1, 1)));
        }

        [Test]
        public void TestPostIncrementAssign2()
        {
            var parameter = Expression.Parameter(typeof(decimal?));
            Expression<Func<decimal?, decimal?>> exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.PostIncrementAssign(parameter, typeof(decimal).GetMethod("op_Increment")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(0m, f(0m));
            Assert.AreEqual(-1m, f(-1m));
            exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.Block(typeof(decimal?), Expression.PostIncrementAssign(parameter, typeof(decimal).GetMethod("op_Increment")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(1m, f(0m));
            Assert.AreEqual(0m, f(-1m));
        }

        [Test]
        public void TestPreDecrementAssign1()
        {
            var parameter = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction>> exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.PreDecrementAssign(parameter, typeof(Fraction).GetMethod("op_Decrement")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(-1, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(1, 1)));
            exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.Block(typeof(Fraction), Expression.PreDecrementAssign(parameter, typeof(Fraction).GetMethod("op_Decrement")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(-1, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(1, 1)));
        }

        [Test]
        public void TestPreDecrementAssign2()
        {
            var parameter = Expression.Parameter(typeof(decimal?));
            Expression<Func<decimal?, decimal?>> exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.PreDecrementAssign(parameter, typeof(decimal).GetMethod("op_Decrement")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(-1m, f(0m));
            Assert.AreEqual(0m, f(1m));
            exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.Block(typeof(decimal?), Expression.PreDecrementAssign(parameter, typeof(decimal).GetMethod("op_Decrement")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(-1m, f(0m));
            Assert.AreEqual(0m, f(1m));
        }

        [Test]
        public void TestPostDecrementAssign1()
        {
            var parameter = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction>> exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.PostDecrementAssign(parameter, typeof(Fraction).GetMethod("op_Decrement")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(1, 1), f(new Fraction(1, 1)));
            exp = Expression.Lambda<Func<Fraction, Fraction>>(Expression.Block(typeof(Fraction), Expression.PostDecrementAssign(parameter, typeof(Fraction).GetMethod("op_Decrement")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(new Fraction(-1, 1), f(new Fraction(0, 1)));
            Assert.AreEqual(new Fraction(0, 1), f(new Fraction(1, 1)));
        }

        [Test]
        public void TestPostDecrementAssign2()
        {
            var parameter = Expression.Parameter(typeof(decimal?));
            Expression<Func<decimal?, decimal?>> exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.PostDecrementAssign(parameter, typeof(decimal).GetMethod("op_Decrement")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(0m, f(0m));
            Assert.AreEqual(1m, f(1m));
            exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.Block(typeof(decimal?), Expression.PostDecrementAssign(parameter, typeof(decimal).GetMethod("op_Decrement")), parameter), parameter);
            f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsNull(f(null));
            Assert.AreEqual(-1m, f(0m));
            Assert.AreEqual(0m, f(1m));
        }

        [Test]
        public void TestUnaryPlus1()
        {
            Expression<Func<Fraction, Fraction>> exp = a => +a;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(new Fraction(1, 3), f(new Fraction(1, 3)));
        }

        [Test]
        public void TestUnaryPlus2()
        {
            var parameter = Expression.Parameter(typeof(decimal?));
            Expression<Func<decimal?, decimal?>> exp = Expression.Lambda<Func<decimal?, decimal?>>(Expression.UnaryPlus(parameter, typeof(decimal).GetMethod("op_UnaryPlus")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(1, f(1));
            Assert.AreEqual(-1, f(-1));
            Assert.IsNull(f(null));
        }

        [Test]
        public void TestUnaryMinus1()
        {
            Expression<Func<Fraction, Fraction>> exp = a => -a;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(new Fraction(-1, 3) == f(new Fraction(1, 3)));
            ParameterExpression parameter = Expression.Parameter(typeof(Fraction));
            Expression<Func<Fraction, Fraction>> exp2 = Expression.Lambda<Func<Fraction, Fraction>>(Expression.NegateChecked(parameter, ((UnaryExpression)exp.Body).Method), parameter);
            f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            Assert.AreEqual(new Fraction(-1, 3), f(new Fraction(1, 3)));
        }

        [Test]
        public void TestUnaryMinus2()
        {
            Expression<Func<decimal?, decimal?>> exp = x => -x;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(0));
            Assert.AreEqual(-1, f(1));
            Assert.AreEqual(1, f(-1));
            Assert.IsNull(f(null));
        }

        [Test]
        public void TestEqual1()
        {
            Expression<Func<Fraction, Fraction, bool>> exp = (a, b) => a == b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(new Fraction(1, 2), new Fraction(1, 2)));
            Assert.IsFalse(f(new Fraction(1, 3), new Fraction(1, 2)));
        }

        [Test]
        public void TestEqual2()
        {
            Expression<Func<decimal?, decimal?, bool>> exp = (a, b) => a == b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(1, 1));
            Assert.IsFalse(f(1, 2));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsTrue(f(null, null));
        }

        [Test]
        public void TestNotEqual1()
        {
            Expression<Func<Fraction, Fraction, bool>> exp = (a, b) => a != b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(new Fraction(1, 2), new Fraction(1, 2)));
            Assert.IsTrue(f(new Fraction(1, 3), new Fraction(1, 2)));
        }

        [Test]
        public void TestNotEqual2()
        {
            Expression<Func<decimal?, decimal?, bool>> exp = (a, b) => a != b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(1, 1));
            Assert.IsTrue(f(1, 2));
            Assert.IsTrue(f(1, null));
            Assert.IsTrue(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestGreaterThan1()
        {
            Expression<Func<Fraction, Fraction, bool>> exp = (a, b) => a > b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(new Fraction(1, 2), new Fraction(1, 2)));
            Assert.IsTrue(f(new Fraction(1, 2), new Fraction(1, 3)));
            Assert.IsFalse(f(new Fraction(1, 3), new Fraction(1, 2)));
        }

        [Test]
        public void TestGreaterThan2()
        {
            Expression<Func<decimal?, decimal?, bool>> exp = (a, b) => a > b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestLessThan1()
        {
            Expression<Func<Fraction, Fraction, bool>> exp = (a, b) => a < b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(new Fraction(1, 2), new Fraction(1, 2)));
            Assert.IsFalse(f(new Fraction(1, 2), new Fraction(1, 3)));
            Assert.IsTrue(f(new Fraction(1, 3), new Fraction(1, 2)));
        }

        [Test]
        public void TestLessThan2()
        {
            Expression<Func<decimal?, decimal?, bool>> exp = (a, b) => a < b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestGreaterThanOrEqual1()
        {
            Expression<Func<Fraction, Fraction, bool>> exp = (a, b) => a >= b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(new Fraction(1, 2), new Fraction(1, 2)));
            Assert.IsTrue(f(new Fraction(1, 2), new Fraction(1, 3)));
            Assert.IsFalse(f(new Fraction(1, 3), new Fraction(1, 2)));
        }

        [Test]
        public void TestLessThanOrEqual2()
        {
            Expression<Func<decimal?, decimal?, bool>> exp = (a, b) => a <= b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(f(3, 1));
            Assert.IsTrue(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestGreaterThanOrEqual2()
        {
            Expression<Func<decimal?, decimal?, bool>> exp = (a, b) => a >= b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(3, 1));
            Assert.IsFalse(f(-3, -1));
            Assert.IsTrue(f(-1, -1));
            Assert.IsFalse(f(1, null));
            Assert.IsFalse(f(null, 1));
            Assert.IsFalse(f(null, null));
        }

        [Test]
        public void TestLessThanOrEqual1()
        {
            Expression<Func<Fraction, Fraction, bool>> exp = (a, b) => a <= b;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(new Fraction(1, 2), new Fraction(1, 2)));
            Assert.IsFalse(f(new Fraction(1, 2), new Fraction(1, 3)));
            Assert.IsTrue(f(new Fraction(1, 3), new Fraction(1, 2)));
        }

        public class Fraction
        {
            public Fraction(int num, int den)
            {
                Num = num;
                Den = den;
                Normalize();
            }

            public bool Equals(Fraction other)
            {
                if(ReferenceEquals(null, other)) return false;
                if(ReferenceEquals(this, other)) return true;
                return other.Num == Num && other.Den == Den;
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj)) return false;
                if(ReferenceEquals(this, obj)) return true;
                if(obj.GetType() != typeof(Fraction)) return false;
                return Equals((Fraction)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Num * 397) ^ Den;
                }
            }

            public static Fraction operator ++(Fraction fraction)
            {
                return fraction + new Fraction(1, 1);
            }

            public static Fraction operator --(Fraction fraction)
            {
                return fraction - new Fraction(1, 1);
            }

            public static Fraction operator +(Fraction fraction)
            {
                return fraction == null ? null : new Fraction(fraction.Num, fraction.Den);
            }

            public static Fraction operator -(Fraction fraction)
            {
                return fraction == null ? null : new Fraction(-fraction.Num, fraction.Den);
            }

            public static Fraction operator +(Fraction left, Fraction right)
            {
                if(left == null || right == null)
                    return null;
                return new Fraction(left.Num * right.Den + left.Den * right.Num, left.Den * right.Den);
            }

            public static Fraction operator -(Fraction left, Fraction right)
            {
                if(left == null || right == null)
                    return null;
                return new Fraction(left.Num * right.Den - left.Den * right.Num, left.Den * right.Den);
            }

            public static Fraction operator *(Fraction left, Fraction right)
            {
                if(left == null || right == null)
                    return null;
                return new Fraction(left.Num * right.Num, left.Den * right.Den);
            }

            public static Fraction operator /(Fraction left, Fraction right)
            {
                if(left == null || right == null)
                    return null;
                return new Fraction(left.Num * right.Den, left.Den * right.Num);
            }

            public static bool operator ==(Fraction left, Fraction right)
            {
                if(ReferenceEquals(left, null) || ReferenceEquals(right, null))
                    return ReferenceEquals(left, null) && ReferenceEquals(right, null);
                return left.Num == right.Num && left.Den == right.Den;
            }

            public static bool operator !=(Fraction left, Fraction right)
            {
                return !(left == right);
            }

            public static bool operator <(Fraction left, Fraction right)
            {
                if(left == null || right == null)
                    return false;
                return left.Num * right.Den < left.Den * right.Num;
            }

            public static bool operator >(Fraction left, Fraction right)
            {
                if(left == null || right == null)
                    return false;
                return left.Num * right.Den > left.Den * right.Num;
            }

            public static bool operator <=(Fraction left, Fraction right)
            {
                if(left == null || right == null)
                    return false;
                return left.Num * right.Den <= left.Den * right.Num;
            }

            public static bool operator >=(Fraction left, Fraction right)
            {
                if(left == null || right == null)
                    return false;
                return left.Num * right.Den >= left.Den * right.Num;
            }

            public int Num { get; private set; }
            public int Den { get; private set; }

            private void Normalize()
            {
                var gcd = Gcd(Num, Den);
                Num /= gcd;
                Den /= gcd;
                if(Den < 0)
                {
                    Num = -Num;
                    Den = -Den;
                }
            }

            private static int Gcd(int a, int b)
            {
                while(b != 0)
                {
                    var r = a % b;
                    a = b;
                    b = r;
                }
                return a;
            }
        }
    }
}