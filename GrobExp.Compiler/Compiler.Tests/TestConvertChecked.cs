using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestConvertChecked
    {
        [Test]
        public void TestInt8Negative()
        {
            Assert.AreEqual(-13, Convert<sbyte, int>(-13));
            Assert.Throws<OverflowException>(() => Convert<sbyte, uint>(-13));
            Assert.AreEqual(-13, Convert<sbyte, sbyte>(-13));
            Assert.Throws<OverflowException>(() => Convert<sbyte, byte>(-13));
            Assert.AreEqual(-13, Convert<sbyte, short>(-13));
            Assert.Throws<OverflowException>(() => Convert<sbyte, ushort>(-13));
            Assert.AreEqual(-13, Convert<sbyte, long>(-13));
            Assert.Throws<OverflowException>(() => Convert<sbyte, ulong>(-13));
            Assert.AreEqual((float)(-13), Convert<sbyte, float>(-13));
            Assert.AreEqual((double)(-13), Convert<sbyte, double>(-13));
        }

        [Test]
        public void TestInt8Positive()
        {
            Assert.AreEqual(13, Convert<sbyte, int>(13));
            Assert.AreEqual((sbyte)13, Convert<sbyte, uint>(13));
            Assert.AreEqual(13, Convert<sbyte, sbyte>(13));
            Assert.AreEqual(13, Convert<sbyte, byte>(13));
            Assert.AreEqual(13, Convert<sbyte, short>(13));
            Assert.AreEqual(13, Convert<sbyte, ushort>(13));
            Assert.AreEqual(13, Convert<sbyte, long>(13));
            Assert.AreEqual((sbyte)13, Convert<sbyte, ulong>(13));
            Assert.AreEqual((float)13, Convert<sbyte, float>(13));
            Assert.AreEqual((double)13, Convert<sbyte, double>(13));
        }

        [Test]
        public void TestInt16Negative()
        {
            Assert.AreEqual(-13, Convert<short, int>(-13));
            Assert.Throws<OverflowException>(() => Convert<short, uint>(-13));
            Assert.AreEqual(-13, Convert<short, sbyte>(-13));
            Assert.Throws<OverflowException>(() => Convert<short, sbyte>(-1000));
            Assert.Throws<OverflowException>(() => Convert<short, byte>(-13));
            Assert.AreEqual(-13, Convert<short, short>(-13));
            Assert.Throws<OverflowException>(() => Convert<short, ushort>(-13));
            Assert.AreEqual(-13, Convert<short, long>(-13));
            Assert.Throws<OverflowException>(() => Convert<short, ulong>(-13));
            Assert.AreEqual((float)(-13), Convert<short, float>(-13));
            Assert.AreEqual((double)(-13), Convert<short, double>(-13));
        }

        [Test]
        public void TestInt16Positive()
        {
            Assert.AreEqual(13, Convert<short, int>(13));
            Assert.AreEqual((short)13, Convert<short, uint>(13));
            Assert.AreEqual(13, Convert<short, sbyte>(13));
            Assert.Throws<OverflowException>(() => Convert<short, sbyte>(200));
            Assert.Throws<OverflowException>(() => Convert<short, byte>(300));
            Assert.AreEqual(13, Convert<short, byte>(13));
            Assert.AreEqual(13, Convert<short, short>(13));
            Assert.AreEqual(13, Convert<short, ushort>(13));
            Assert.AreEqual(13, Convert<short, long>(13));
            Assert.AreEqual((short)13, Convert<short, ulong>(13));
            Assert.AreEqual((float)13, Convert<short, float>(13));
            Assert.AreEqual((double)13, Convert<short, double>(13));
        }

        [Test]
        public void TestInt32Negative()
        {
            Assert.AreEqual(-13, Convert<int, int>(-13));
            Assert.Throws<OverflowException>(() => Convert<int, uint>(-13));
            Assert.AreEqual(-13, Convert<int, sbyte>(-13));
            Assert.Throws<OverflowException>(() => Convert<int, byte>(-13));
            Assert.Throws<OverflowException>(() => Convert<int, sbyte>(-130));
            Assert.AreEqual(-13, Convert<int, short>(-13));
            Assert.Throws<OverflowException>(() => Convert<int, ushort>(-13));
            Assert.Throws<OverflowException>(() => Convert<int, short>(-33000));
            Assert.AreEqual(-13, Convert<int, long>(-13));
            Assert.Throws<OverflowException>(() => Convert<int, ulong>(-13));
            Assert.AreEqual((float)(-13), Convert<int, float>(-13));
            Assert.AreEqual((double)(-13), Convert<int, double>(-13));
        }

        [Test]
        public void TestInt32Positive()
        {
            Assert.AreEqual(13, Convert<int, int>(13));
            Assert.AreEqual(13, Convert<int, uint>(13));
            Assert.AreEqual(13, Convert<int, sbyte>(13));
            Assert.Throws<OverflowException>(() => Convert<int, sbyte>(200));
            Assert.AreEqual(13, Convert<int, byte>(13));
            Assert.Throws<OverflowException>(() => Convert<int, sbyte>(300));
            Assert.AreEqual(13, Convert<int, short>(13));
            Assert.Throws<OverflowException>(() => Convert<int, short>(33000));
            Assert.AreEqual(13, Convert<int, ushort>(13));
            Assert.Throws<OverflowException>(() => Convert<int, short>(66000));
            Assert.AreEqual(13, Convert<int, long>(13));
            Assert.AreEqual(13, Convert<int, ulong>(13));
            Assert.AreEqual((float)13, Convert<int, float>(13));
            Assert.AreEqual((double)13, Convert<int, double>(13));
        }

        [Test]
        public void TestUInt8()
        {
            Assert.AreEqual(250, Convert<byte, int>(250));
            Assert.AreEqual(250, Convert<byte, uint>(250));
            Assert.Throws<OverflowException>(() => Convert<byte, sbyte>(250));
            Assert.AreEqual(250, Convert<byte, byte>(250));
            Assert.AreEqual(250, Convert<byte, short>(250));
            Assert.AreEqual(250, Convert<byte, ushort>(250));
            Assert.AreEqual(250, Convert<byte, long>(250));
            Assert.AreEqual(250, Convert<byte, ulong>(250));
            Assert.AreEqual((float)250, Convert<byte, float>(250));
            Assert.AreEqual((double)250, Convert<byte, double>(250));
        }

        [Test]
        public void TestUInt16()
        {
            Assert.AreEqual(54321, Convert<ushort, int>(54321));
            Assert.AreEqual(54321, Convert<ushort, uint>(54321));
            Assert.AreEqual(13, Convert<ushort, sbyte>(13));
            Assert.Throws<OverflowException>(() => Convert<ushort, sbyte>(200));
            Assert.AreEqual(13, Convert<ushort, byte>(13));
            Assert.Throws<OverflowException>(() => Convert<ushort, sbyte>(300));
            Assert.Throws<OverflowException>(() => Convert<ushort, short>(54321));
            Assert.AreEqual(54321, Convert<ushort, ushort>(54321));
            Assert.AreEqual(54321, Convert<ushort, long>(54321));
            Assert.AreEqual(54321, Convert<ushort, ulong>(54321));
            Assert.AreEqual((float)54321, Convert<ushort, float>(54321));
            Assert.AreEqual((double)54321, Convert<ushort, double>(54321));
        }

        [Test]
        public void TestUInt32()
        {
            Assert.Throws<OverflowException>(() => Convert<uint, int>(3000000000));
            Assert.AreEqual(3000000000, Convert<uint, uint>(3000000000));
            Assert.AreEqual(13, Convert<uint, sbyte>(13));
            Assert.Throws<OverflowException>(() => Convert<uint, sbyte>(200));
            Assert.AreEqual(13, Convert<uint, byte>(13));
            Assert.Throws<OverflowException>(() => Convert<uint, byte>(300));
            Assert.AreEqual(1000, Convert<uint, short>(1000));
            Assert.Throws<OverflowException>(() => Convert<uint, short>(33000));
            Assert.AreEqual(1000, Convert<uint, ushort>(1000));
            Assert.Throws<OverflowException>(() => Convert<uint, ushort>(66000));
            Assert.AreEqual(3000000000, Convert<uint, long>(3000000000));
            Assert.AreEqual(3000000000, Convert<uint, ulong>(3000000000));
            Assert.AreEqual((float)3000000000, Convert<uint, float>(3000000000));
            Assert.AreEqual((double)3000000000, Convert<uint, double>(3000000000));
        }

        [Test]
        public void TestUInt64()
        {
            unchecked
            {
                Assert.AreEqual(13, Convert<ulong, sbyte>(13));
                Assert.Throws<OverflowException>(() => Convert<ulong, sbyte>(200));
                Assert.AreEqual(13, Convert<ulong, byte>(13));
                Assert.Throws<OverflowException>(() => Convert<ulong, byte>(300));
                Assert.AreEqual(1000, Convert<ulong, short>(1000));
                Assert.Throws<OverflowException>(() => Convert<ulong, short>(33000));
                Assert.AreEqual(1000, Convert<ulong, ushort>(1000));
                Assert.Throws<OverflowException>(() => Convert<ulong, ushort>(66000));
                Assert.AreEqual(1000000000, Convert<ulong, int>(1000000000));
                Assert.Throws<OverflowException>(() => Convert<ulong, int>(3000000000));
                Assert.AreEqual(1000000000, Convert<ulong, uint>(1000000000));
                Assert.Throws<OverflowException>(() => Convert<ulong, uint>(5000000000));
                Assert.Throws<OverflowException>(() => Convert<ulong, long>(10000000000000000000));
                Assert.AreEqual(10000000000000000000, Convert<ulong, ulong>(10000000000000000000));
                Assert.AreEqual((float)10000000000000000000, Convert<ulong, float>(10000000000000000000));
                Assert.AreEqual((double)10000000000000000000, Convert<ulong, double>(10000000000000000000));
            }
        }

        [Test]
        public void TestInt64Positive()
        {
            Assert.AreEqual(13, Convert<long, sbyte>(13));
            Assert.Throws<OverflowException>(() => Convert<long, sbyte>(200));
            Assert.AreEqual(13, Convert<long, byte>(13));
            Assert.Throws<OverflowException>(() => Convert<long, byte>(300));
            Assert.AreEqual(1000, Convert<long, short>(1000));
            Assert.Throws<OverflowException>(() => Convert<long, short>(33000));
            Assert.AreEqual(1000, Convert<long, ushort>(1000));
            Assert.Throws<OverflowException>(() => Convert<long, ushort>(66000));
            Assert.AreEqual(1000000000, Convert<long, int>(1000000000));
            Assert.Throws<OverflowException>(() => Convert<long, int>(3000000000));
            Assert.AreEqual(1000000000, Convert<long, uint>(1000000000));
            Assert.Throws<OverflowException>(() => Convert<long, uint>(5000000000));
            Assert.AreEqual(1000000000000000000, Convert<long, long>(1000000000000000000));
            Assert.AreEqual(1000000000000000000, Convert<long, ulong>(1000000000000000000));
            Assert.AreEqual((float)1000000000000000000, Convert<long, float>(1000000000000000000));
            Assert.AreEqual((double)1000000000000000000, Convert<long, double>(1000000000000000000));
        }

        [Test]
        public void TestInt64Negative()
        {
            unchecked
            {
                Assert.AreEqual((long)(-13), Convert<long, int>(-13));
                Assert.Throws<OverflowException>(() => Convert<long, int>(-3000000000));
                Assert.Throws<OverflowException>(() => Convert<long, uint>(-13));
                Assert.AreEqual((long)(-13), Convert<long, sbyte>(-13));
                Assert.Throws<OverflowException>(() => Convert<long, sbyte>(-130));
                Assert.Throws<OverflowException>(() => Convert<long, byte>(-13));
                Assert.AreEqual((long)(-13), Convert<long, short>(-13));
                Assert.Throws<OverflowException>(() => Convert<long, short>(-33000));
                Assert.Throws<OverflowException>(() => Convert<long, ushort>(-13));
                Assert.AreEqual(-13, Convert<long, long>(-13));
                Assert.Throws<OverflowException>(() => Convert<long, ulong>(-13));
                Assert.AreEqual((float)(-13), Convert<long, float>(-13));
                Assert.AreEqual((double)(-13), Convert<long, double>(-13));
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

        private TOut Convert<TIn, TOut>(TIn value)
        {
            var parameter = Expression.Parameter(typeof(TIn));
            Expression<Func<TIn, TOut>> exp = Expression.Lambda<Func<TIn, TOut>>(Expression.ConvertChecked(parameter, typeof(TOut)), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            return f(value);
        }
    }
}