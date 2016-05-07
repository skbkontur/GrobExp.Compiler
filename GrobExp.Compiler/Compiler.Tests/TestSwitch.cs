using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestSwitch
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int));
            var exp = Expression.Lambda<Func<int, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant(0), Expression.Constant(2)),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant(5), Expression.Constant(1000001)),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant(7), Expression.Constant(1000000))
                    ),
                a
                );
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(0));
            Assert.AreEqual("xxx", f(1));
            Assert.AreEqual("zzz", f(2));
            Assert.AreEqual("xxx", f(3));
            Assert.AreEqual("qxx", f(5));
            Assert.AreEqual("xxx", f(6));
            Assert.AreEqual("qzz", f(7));
            Assert.AreEqual("xxx", f(123456));
            Assert.AreEqual("qzz", f(1000000));
            Assert.AreEqual("qxx", f(1000001));
        }

        [Test]
        public void TestNullableInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?));
            var exp = Expression.Lambda<Func<int?, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant(null, typeof(int?)), Expression.Constant((int?)0, typeof(int?)), Expression.Constant((int?)2, typeof(int?))),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant((int?)5, typeof(int?)), Expression.Constant((int?)1000001, typeof(int?))),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant((int?)7, typeof(int?)), Expression.Constant((int?)1000000, typeof(int?)))
                    ),
                a
                );
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(null));
            Assert.AreEqual("zzz", f(0));
            Assert.AreEqual("xxx", f(1));
            Assert.AreEqual("zzz", f(2));
            Assert.AreEqual("xxx", f(3));
            Assert.AreEqual("qxx", f(5));
            Assert.AreEqual("xxx", f(6));
            Assert.AreEqual("qzz", f(7));
            Assert.AreEqual("xxx", f(123456));
            Assert.AreEqual("qzz", f(1000000));
            Assert.AreEqual("qxx", f(1000001));
        }

        [Test]
        public void TestSbyte()
        {
            ParameterExpression a = Expression.Parameter(typeof(sbyte));
            var exp = Expression.Lambda<Func<sbyte, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant((sbyte)0), Expression.Constant((sbyte)2)),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant((sbyte)5), Expression.Constant((sbyte)-101)),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant((sbyte)7), Expression.Constant((sbyte)-100))
                    ),
                a
                );
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(0));
            Assert.AreEqual("xxx", f(1));
            Assert.AreEqual("zzz", f(2));
            Assert.AreEqual("xxx", f(3));
            Assert.AreEqual("qxx", f(5));
            Assert.AreEqual("xxx", f(6));
            Assert.AreEqual("qzz", f(7));
            Assert.AreEqual("xxx", f(100));
            Assert.AreEqual("qzz", f(-100));
            Assert.AreEqual("qxx", f(-101));
        }

        [Test]
        public void TestByte()
        {
            ParameterExpression a = Expression.Parameter(typeof(byte));
            var exp = Expression.Lambda<Func<byte, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant((byte)0), Expression.Constant((byte)2)),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant((byte)5), Expression.Constant((byte)101)),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant((byte)7), Expression.Constant((byte)100))
                    ),
                a
                );
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(0));
            Assert.AreEqual("xxx", f(1));
            Assert.AreEqual("zzz", f(2));
            Assert.AreEqual("xxx", f(3));
            Assert.AreEqual("qxx", f(5));
            Assert.AreEqual("xxx", f(6));
            Assert.AreEqual("qzz", f(7));
            Assert.AreEqual("xxx", f(200));
            Assert.AreEqual("qzz", f(100));
            Assert.AreEqual("qxx", f(101));
        }

        [Test]
        public void TestLong()
        {
            ParameterExpression a = Expression.Parameter(typeof(long));
            var exp = Expression.Lambda<Func<long, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant((long)0), Expression.Constant((long)2)),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant((long)5), Expression.Constant(-101000000000)),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant((long)7), Expression.Constant(-100000000000))
                    ),
                a
                );
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(0));
            Assert.AreEqual("xxx", f(1));
            Assert.AreEqual("zzz", f(2));
            Assert.AreEqual("xxx", f(3));
            Assert.AreEqual("qxx", f(5));
            Assert.AreEqual("xxx", f(6));
            Assert.AreEqual("qzz", f(7));
            Assert.AreEqual("xxx", f(200000000000));
            Assert.AreEqual("qzz", f(-100000000000));
            Assert.AreEqual("qxx", f(-101000000000));
        }

        [Test]
        public void TestString()
        {
            ParameterExpression a = Expression.Parameter(typeof(string));
            var exp = Expression.Lambda<Func<string, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant("0"), Expression.Constant("2"), Expression.Constant(null, typeof(string))),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant("5"), Expression.Constant("1000001")),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant("7"), Expression.Constant("1000000"))
                    ),
                a
                );
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(null));
            Assert.AreEqual("zzz", f("0"));
            Assert.AreEqual("xxx", f("1"));
            Assert.AreEqual("zzz", f("2"));
            Assert.AreEqual("xxx", f("3"));
            Assert.AreEqual("qxx", f("5"));
            Assert.AreEqual("xxx", f("6"));
            Assert.AreEqual("qzz", f("7"));
            Assert.AreEqual("xxx", f("123456"));
            Assert.AreEqual("qzz", f("1000000"));
            Assert.AreEqual("qxx", f("1000001"));
        }

        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double));
            var exp = Expression.Lambda<Func<double, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant(0.0), Expression.Constant(2.0)),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant(5.0), Expression.Constant(1000001.0)),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant(7.0), Expression.Constant(1000000.0))
                    ),
                a
                );
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(0.0));
            Assert.AreEqual("xxx", f(1.0));
            Assert.AreEqual("zzz", f(2.0));
            Assert.AreEqual("xxx", f(3.0));
            Assert.AreEqual("qxx", f(5.0));
            Assert.AreEqual("xxx", f(6.0));
            Assert.AreEqual("qzz", f(7.0));
            Assert.AreEqual("xxx", f(123456.0));
            Assert.AreEqual("qzz", f(1000000.0));
            Assert.AreEqual("qxx", f(1000001.0));
        }

        [Test]
        public void TestNullableDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(double?));
            var exp = Expression.Lambda<Func<double?, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant(null, typeof(double?)), Expression.Constant((double?)0.0, typeof(double?)), Expression.Constant((double?)2.0, typeof(double?))),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant((double?)5.0, typeof(double?)), Expression.Constant((double?)1000001.0, typeof(double?))),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant((double?)7.0, typeof(double?)), Expression.Constant((double?)1000000.0, typeof(double?)))
                    ),
                a
                );
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f(null));
            Assert.AreEqual("zzz", f(0.0));
            Assert.AreEqual("xxx", f(1.0));
            Assert.AreEqual("zzz", f(2.0));
            Assert.AreEqual("xxx", f(3.0));
            Assert.AreEqual("qxx", f(5.0));
            Assert.AreEqual("xxx", f(6.0));
            Assert.AreEqual("qzz", f(7.0));
            Assert.AreEqual("xxx", f(123456.0));
            Assert.AreEqual("qzz", f(1000000.0));
            Assert.AreEqual("qxx", f(1000001.0));
        }
    }
}