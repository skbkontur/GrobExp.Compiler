using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestConstant
    {
        [Test]
        public void TestBool()
        {
            Expression<Func<bool>> exp = Expression.Lambda<Func<bool>>(Expression.Constant(true, typeof(bool)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(true, f());
        }

        [Test]
        public void TestByte()
        {
            Expression<Func<byte>> exp = Expression.Lambda<Func<byte>>(Expression.Constant((byte)250, typeof(byte)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(250, f());
        }

        [Test]
        public void TestSByte()
        {
            Expression<Func<sbyte>> exp = Expression.Lambda<Func<sbyte>>(Expression.Constant((sbyte)-123, typeof(sbyte)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-123, f());
        }

        [Test]
        public void TestUShort()
        {
            Expression<Func<ushort>> exp = Expression.Lambda<Func<ushort>>(Expression.Constant((ushort)65500, typeof(ushort)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(65500, f());
        }

        [Test]
        public void TestShort()
        {
            Expression<Func<short>> exp = Expression.Lambda<Func<short>>(Expression.Constant((short)-32000, typeof(short)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-32000, f());
        }

        [Test]
        public void TestUInt()
        {
            Expression<Func<uint>> exp = Expression.Lambda<Func<uint>>(Expression.Constant(3500000000, typeof(uint)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(3500000000, f());
        }

        [Test]
        public void TestInt()
        {
            Expression<Func<int>> exp = Expression.Lambda<Func<int>>(Expression.Constant(-2000000000, typeof(int)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-2000000000, f());
        }

        [Test]
        public void TestULong()
        {
            Expression<Func<ulong>> exp = Expression.Lambda<Func<ulong>>(Expression.Constant(ulong.MaxValue, typeof(ulong)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(ulong.MaxValue, f());
        }

        [Test]
        public void TestLong()
        {
            Expression<Func<long>> exp = Expression.Lambda<Func<long>>(Expression.Constant(long.MinValue, typeof(long)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(long.MinValue, f());
        }

        [Test]
        public void TestFloat()
        {
            Expression<Func<float>> exp = Expression.Lambda<Func<float>>(Expression.Constant(float.Epsilon, typeof(float)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(float.Epsilon, f());
        }

        [Test]
        public void TestDouble()
        {
            Expression<Func<double>> exp = Expression.Lambda<Func<double>>(Expression.Constant(double.Epsilon, typeof(double)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(double.Epsilon, f());
        }

        [Test]
        public void TestString()
        {
            Expression<Func<string>> exp = Expression.Lambda<Func<string>>(Expression.Constant("zzz", typeof(string)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f());
        }
    }
}