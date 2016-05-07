using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestConvert
    {
        [Test]
        public void TestFromDecimal()
        {
            unchecked
            {
                Assert.AreEqual(13, ConvertFromDecimal(d => (sbyte)d, 13.123m));
                Assert.AreEqual(13, ConvertFromDecimal(d => (byte)d, 13.123m));
                Assert.AreEqual(1000, ConvertFromDecimal(d => (short)d, 1000.382456m));
                Assert.AreEqual(1000, ConvertFromDecimal(d => (ushort)d, 1000.382456m));
                Assert.AreEqual(1000000000, ConvertFromDecimal(d => (int)d, 1000000000.73465m));
                Assert.AreEqual(1000000000, ConvertFromDecimal(d => (uint)d, 1000000000.73465m));
                Assert.AreEqual(12345678912345678, ConvertFromDecimal(d => (long)d, 12345678912345678.73465m));
                Assert.AreEqual(12345678912345678, ConvertFromDecimal(d => (ulong)d, 12345678912345678.73465m));
                Assert.AreEqual((float)3.1415926, ConvertFromDecimal(d => (float)d, 3.1415926m));
                Assert.AreEqual(3.1415926, ConvertFromDecimal(d => (double)d, 3.1415926m));
                Assert.AreEqual(3.1415926m, ConvertFromDecimal(d => d, 3.1415926m));
            }
        }

        [Test]
        public void TestToDecimal()
        {
            unchecked
            {
                Assert.AreEqual(-13m, ConvertToDecimal<sbyte>(x => (decimal)x, -13));
                Assert.AreEqual(13m, ConvertToDecimal<sbyte>(x => (decimal)x, 13));
                Assert.AreEqual(-13m, ConvertToDecimal<short>(x => (decimal)x, -13));
                Assert.AreEqual(13m, ConvertToDecimal<short>(x => (decimal)x, 13));
                Assert.AreEqual(-13m, ConvertToDecimal(x => (decimal)x, -13));
                Assert.AreEqual(13m, ConvertToDecimal(x => (decimal)x, 13));
                Assert.AreEqual(250m, ConvertToDecimal<byte>(x => (decimal)x, 250));
                Assert.AreEqual(54321m, ConvertToDecimal<ushort>(x => (decimal)x, 54321));
                Assert.AreEqual(3000000000m, ConvertToDecimal(x => (decimal)x, 3000000000));
                Assert.AreEqual(10000000000000000000m, ConvertToDecimal(x => (decimal)x, 10000000000000000000));
                Assert.AreEqual(1000000000000000000m, ConvertToDecimal(x => (decimal)x, 1000000000000000000));
                Assert.AreEqual(-13m, ConvertToDecimal<long>(x => (decimal)x, -13));
                Assert.AreEqual((decimal)3.1415926f, ConvertToDecimal(x => (decimal)x, 3.1415926f));
                Assert.AreEqual((decimal)3.1415926, ConvertToDecimal(x => (decimal)x, 3.1415926));

                Assert.AreEqual(13, ConvertFromDecimal(d => (sbyte)d, 13.123m));
                Assert.AreEqual(13, ConvertFromDecimal(d => (byte)d, 13.123m));
                Assert.AreEqual(1000, ConvertFromDecimal(d => (short)d, 1000.382456m));
                Assert.AreEqual(1000, ConvertFromDecimal(d => (ushort)d, 1000.382456m));
                Assert.AreEqual(1000000000, ConvertFromDecimal(d => (int)d, 1000000000.73465m));
                Assert.AreEqual(1000000000, ConvertFromDecimal(d => (uint)d, 1000000000.73465m));
                Assert.AreEqual(12345678912345678, ConvertFromDecimal(d => (long)d, 12345678912345678.73465m));
                Assert.AreEqual(12345678912345678, ConvertFromDecimal(d => (ulong)d, 12345678912345678.73465m));
                Assert.AreEqual((float)3.1415926, ConvertFromDecimal(d => (float)d, 3.1415926m));
                Assert.AreEqual(3.1415926, ConvertFromDecimal(d => (double)d, 3.1415926m));
                Assert.AreEqual(3.1415926m, ConvertFromDecimal(d => d, 3.1415926m));
            }
        }

        [Test]
        public void TestInt8Negative()
        {
            const sbyte x = -13;
            const byte y = -x - 1;
            Assert.AreEqual(x, Convert<sbyte, int>(x));
            Assert.AreEqual(uint.MaxValue - y, Convert<sbyte, uint>(x));
            Assert.AreEqual(x, Convert<sbyte, sbyte>(x));
            Assert.AreEqual(byte.MaxValue - y, Convert<sbyte, byte>(x));
            Assert.AreEqual(x, Convert<sbyte, short>(x));
            Assert.AreEqual(ushort.MaxValue - y, Convert<sbyte, ushort>(x));
            Assert.AreEqual(x, Convert<sbyte, long>(x));
            Assert.AreEqual(ulong.MaxValue - y, Convert<sbyte, ulong>(x));
            Assert.AreEqual((float)x, Convert<sbyte, float>(x));
            Assert.AreEqual((double)x, Convert<sbyte, double>(x));
        }

        [Test]
        public void TestInt8Positive()
        {
            const sbyte x = 13;
            Assert.AreEqual(x, Convert<sbyte, int>(x));
            Assert.AreEqual(x, Convert<sbyte, uint>(x));
            Assert.AreEqual(x, Convert<sbyte, sbyte>(x));
            Assert.AreEqual(x, Convert<sbyte, byte>(x));
            Assert.AreEqual(x, Convert<sbyte, short>(x));
            Assert.AreEqual(x, Convert<sbyte, ushort>(x));
            Assert.AreEqual(x, Convert<sbyte, long>(x));
            Assert.AreEqual(x, Convert<sbyte, ulong>(x));
            Assert.AreEqual((float)x, Convert<sbyte, float>(x));
            Assert.AreEqual((double)x, Convert<sbyte, double>(x));
        }

        [Test]
        public void TestInt16Negative()
        {
            const short x = -13;
            const ushort y = -x - 1;
            Assert.AreEqual(x, Convert<short, int>(x));
            Assert.AreEqual(uint.MaxValue - y, Convert<short, uint>(x));
            Assert.AreEqual(x, Convert<short, sbyte>(x));
            Assert.AreEqual(byte.MaxValue - y, Convert<short, byte>(x));
            Assert.AreEqual(x, Convert<short, short>(x));
            Assert.AreEqual(ushort.MaxValue - y, Convert<short, ushort>(x));
            Assert.AreEqual(x, Convert<short, long>(x));
            Assert.AreEqual(ulong.MaxValue - y, Convert<short, ulong>(x));
            Assert.AreEqual((float)x, Convert<short, float>(x));
            Assert.AreEqual((double)x, Convert<short, double>(x));
        }

        [Test]
        public void TestInt16Positive()
        {
            const short x = 13;
            Assert.AreEqual(x, Convert<short, int>(x));
            Assert.AreEqual(x, Convert<short, uint>(x));
            Assert.AreEqual(x, Convert<short, sbyte>(x));
            Assert.AreEqual(x, Convert<short, byte>(x));
            Assert.AreEqual(x, Convert<short, short>(x));
            Assert.AreEqual(x, Convert<short, ushort>(x));
            Assert.AreEqual(x, Convert<short, long>(x));
            Assert.AreEqual(x, Convert<short, ulong>(x));
            Assert.AreEqual((float)x, Convert<short, float>(x));
            Assert.AreEqual((double)x, Convert<short, double>(x));
        }

        [Test]
        public void TestInt32Negative()
        {
            const int x = -13;
            const uint y = -x - 1;
            Assert.AreEqual(x, Convert<int, int>(x));
            Assert.AreEqual(uint.MaxValue - y, Convert<int, uint>(x));
            Assert.AreEqual(x, Convert<int, sbyte>(x));
            Assert.AreEqual(byte.MaxValue - y, Convert<int, byte>(x));
            Assert.AreEqual(x, Convert<int, short>(x));
            Assert.AreEqual(ushort.MaxValue - y, Convert<int, ushort>(x));
            Assert.AreEqual(x, Convert<int, long>(x));
            Assert.AreEqual(ulong.MaxValue - y, Convert<int, ulong>(x));
            Assert.AreEqual((float)x, Convert<int, float>(x));
            Assert.AreEqual((double)x, Convert<int, double>(x));
        }

        [Test]
        public void TestInt32Positive()
        {
            const int x = 13;
            Assert.AreEqual(x, Convert<int, int>(x));
            Assert.AreEqual(x, Convert<int, uint>(x));
            Assert.AreEqual(x, Convert<int, sbyte>(x));
            Assert.AreEqual(x, Convert<int, byte>(x));
            Assert.AreEqual(x, Convert<int, short>(x));
            Assert.AreEqual(x, Convert<int, ushort>(x));
            Assert.AreEqual(x, Convert<int, long>(x));
            Assert.AreEqual(x, Convert<int, ulong>(x));
            Assert.AreEqual((float)x, Convert<int, float>(x));
            Assert.AreEqual((double)x, Convert<int, double>(x));
        }

        [Test]
        public void TestUInt8()
        {
            const byte x = 250;
            Assert.AreEqual(x, Convert<byte, int>(x));
            Assert.AreEqual(x, Convert<byte, uint>(x));
            Assert.AreEqual(-(byte.MaxValue - x + 1), Convert<byte, sbyte>(x));
            Assert.AreEqual(x, Convert<byte, byte>(x));
            Assert.AreEqual(x, Convert<byte, short>(x));
            Assert.AreEqual(x, Convert<byte, ushort>(x));
            Assert.AreEqual(x, Convert<byte, long>(x));
            Assert.AreEqual(x, Convert<byte, ulong>(x));
            Assert.AreEqual((float)x, Convert<byte, float>(x));
            Assert.AreEqual((double)x, Convert<byte, double>(x));
        }

        [Test]
        public void TestUInt16()
        {
            const ushort x = 54321;
            Assert.AreEqual(x, Convert<ushort, int>(x));
            Assert.AreEqual(x, Convert<ushort, uint>(x));
            Assert.AreEqual(13, Convert<ushort, sbyte>(13));
            Assert.AreEqual(13, Convert<ushort, byte>(13));
            Assert.AreEqual(-(ushort.MaxValue - x + 1), Convert<ushort, short>(x));
            Assert.AreEqual(x, Convert<ushort, ushort>(x));
            Assert.AreEqual(x, Convert<ushort, long>(x));
            Assert.AreEqual(x, Convert<ushort, ulong>(x));
            Assert.AreEqual((float)x, Convert<ushort, float>(x));
            Assert.AreEqual((double)x, Convert<ushort, double>(x));
        }

        [Test]
        public void TestUInt32()
        {
            const uint x = 3000000000;
            Assert.AreEqual(-(uint.MaxValue - x + 1), Convert<uint, int>(x));
            Assert.AreEqual(x, Convert<uint, uint>(x));
            Assert.AreEqual(13, Convert<uint, sbyte>(13));
            Assert.AreEqual(13, Convert<uint, byte>(13));
            Assert.AreEqual(1000, Convert<uint, short>(1000));
            Assert.AreEqual(1000, Convert<uint, ushort>(1000));
            Assert.AreEqual(x, Convert<uint, long>(x));
            Assert.AreEqual(x, Convert<uint, ulong>(x));
            Assert.AreEqual((float)x, Convert<uint, float>(x));
            Assert.AreEqual((double)x, Convert<uint, double>(x));
        }

        [Test]
        public void TestUInt64()
        {
            unchecked
            {
                const ulong x = 10000000000000000000;
                Assert.AreEqual(13, Convert<ulong, sbyte>(13));
                Assert.AreEqual(13, Convert<ulong, byte>(13));
                Assert.AreEqual(1000, Convert<ulong, short>(1000));
                Assert.AreEqual(1000, Convert<ulong, ushort>(1000));
                Assert.AreEqual(1000000000, Convert<ulong, int>(1000000000));
                Assert.AreEqual(1000000000, Convert<ulong, uint>(1000000000));
                Assert.AreEqual((long)(0L - (ulong.MaxValue - x + 1)), Convert<ulong, long>(x));
                Assert.AreEqual(x, Convert<ulong, ulong>(x));
                Assert.AreEqual((float)x, Convert<ulong, float>(x));
                Assert.AreEqual((double)x, Convert<ulong, double>(x));
            }
        }

        [Test]
        public void TestInt64Positive()
        {
            unchecked
            {
                const long x = 1000000000000000000;
                Assert.AreEqual(13, Convert<long, sbyte>(13));
                Assert.AreEqual(13, Convert<long, byte>(13));
                Assert.AreEqual(1000, Convert<long, short>(1000));
                Assert.AreEqual(1000, Convert<long, ushort>(1000));
                Assert.AreEqual(1000000000, Convert<long, int>(1000000000));
                Assert.AreEqual(1000000000, Convert<long, uint>(1000000000));
                Assert.AreEqual(x, Convert<long, long>(x));
                Assert.AreEqual(x, Convert<long, ulong>(x));
                Assert.AreEqual((float)x, Convert<long, float>(x));
                Assert.AreEqual((double)x, Convert<long, double>(x));
            }
        }

        [Test]
        public void TestInt64Negative()
        {
            unchecked
            {
                const long x = -13;
                const ulong y = -x - 1;
                Assert.AreEqual(x, Convert<long, int>(x));
                Assert.AreEqual(uint.MaxValue - y, Convert<long, uint>(x));
                Assert.AreEqual(x, Convert<long, sbyte>(x));
                Assert.AreEqual(byte.MaxValue - y, Convert<long, byte>(x));
                Assert.AreEqual(x, Convert<long, short>(x));
                Assert.AreEqual(ushort.MaxValue - y, Convert<long, ushort>(x));
                Assert.AreEqual(x, Convert<long, long>(x));
                Assert.AreEqual(ulong.MaxValue - y, Convert<long, ulong>(x));
                Assert.AreEqual((float)x, Convert<long, float>(x));
                Assert.AreEqual((double)x, Convert<long, double>(x));
            }
        }

        [Test]
        public void TestFloat()
        {
            unchecked
            {
                Assert.AreEqual(13, Convert<float, sbyte>(13.123f));
                Assert.AreEqual(13, Convert<float, byte>(13.123f));
                Assert.AreEqual(1000, Convert<float, short>(1000.382456f));
                Assert.AreEqual(1000, Convert<float, ushort>(1000.382456f));
                Assert.AreEqual(1000000000, Convert<float, int>(1000000000.73465f));
                Assert.AreEqual(1000000000, Convert<float, uint>(1000000000.73465f));
                Assert.AreEqual(10000000000, Convert<float, long>(10000000000.73465f));
                Assert.AreEqual(10000000000, Convert<float, ulong>(10000000000.73465f));
                Assert.AreEqual(3.1415926f, Convert<float, float>(3.1415926f));
                Assert.AreEqual((double)3.1415926f, Convert<float, double>(3.1415926f));
            }
        }

        [Test]
        public void TestDouble()
        {
            unchecked
            {
                Assert.AreEqual(13, Convert<double, sbyte>(13.123));
                Assert.AreEqual(13, Convert<double, byte>(13.123));
                Assert.AreEqual(1000, Convert<double, short>(1000.382456));
                Assert.AreEqual(1000, Convert<double, ushort>(1000.382456));
                Assert.AreEqual(1000000000, Convert<double, int>(1000000000.73465));
                Assert.AreEqual(1000000000, Convert<double, uint>(1000000000.73465));
                Assert.AreEqual(12345678912345678, Convert<double, long>(12345678912345678.73465));
                Assert.AreEqual(12345678912345678, Convert<double, ulong>(12345678912345678.73465));
                Assert.AreEqual((float)3.1415926, Convert<double, float>(3.1415926));
                Assert.AreEqual(3.1415926, Convert<double, double>(3.1415926));
            }
        }

        [Test]
        public void TestObject()
        {
            Assert.AreEqual(1, Convert<object, int>(1));
            Assert.AreEqual(1.125, Convert<object, double>(1.125));
            Assert.Throws<NullReferenceException>(() => Convert<object, int>(null));
            Expression<Func<TestClassA, int>> exp = a => (int)a.X;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.Throws<NullReferenceException>(() => f(null));
            Assert.Throws<NullReferenceException>(() => f(new TestClassA()));
        }

        [Test]
        public void TestNullableDecimal()
        {
            decimal? x = 1.0m;
            Assert.AreEqual(1.0, Convert<decimal?, double?>(x));
            var parameter = Expression.Parameter(typeof(object));
            var body = Expression.Convert(Expression.Convert(Expression.Convert(parameter, typeof(int)), typeof(decimal?)), typeof(object));
            var exp = Expression.Lambda<Func<object, object>>(body, parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1.0m, f(1));
        }

        private T ConvertFromDecimal<T>(Expression<Func<decimal, T>> exp, decimal value)
        {
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            return f(value);
        }

        private decimal ConvertToDecimal<T>(Expression<Func<T, decimal>> exp, T value)
        {
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            return f(value);
        }

        private TOut Convert<TIn, TOut>(TIn value)
        {
            var parameter = Expression.Parameter(typeof(TIn));
            Expression<Func<TIn, TOut>> exp = Expression.Lambda<Func<TIn, TOut>>(Expression.Convert(parameter, typeof(TOut)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            return f(value);
        }

        public class TestClassA
        {
            public object X { get; set; }
        }
    }
}