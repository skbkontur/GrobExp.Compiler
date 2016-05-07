using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PowerAssign
{
    [TestFixture]
    public class TestComplexProperty
    {
        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double), "b");
            Expression<Func<TestClassA, double, double>> exp = Expression.Lambda<Func<TestClassA, double, double>>(Expression.PowerAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleArray")), typeof(DoubleArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleArray = new DoubleArray()};
            o.DoubleArray["zzz", 1] = 0;
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleArray["zzz", 1]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleArray = new DoubleArray()};
            o.DoubleArray["zzz", 1] = 0;
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.DoubleArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(double?), "b");
            Expression<Func<TestClassA, double?, double?>> exp = Expression.Lambda<Func<TestClassA, double?, double?>>(Expression.PowerAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableDoubleArray")), typeof(NullableIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableDoubleArray = new NullableIntArray()};
            o.NullableDoubleArray["zzz", 1] = 0;
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.NullableDoubleArray["zzz", 1]);
            o.NullableDoubleArray["zzz", 1] = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleArray["zzz", 1]);
            o.NullableDoubleArray["zzz", 1] = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.NullableDoubleArray["zzz", 1]);
            o.NullableDoubleArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleArray["zzz", 1]);
            Assert.IsNull(f(null, 1));
            o.NullableDoubleArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableDoubleArray["zzz", 1]);
            o.NullableDoubleArray["zzz", 1] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleArray["zzz", 1]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleArray["zzz", 1]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableDoubleArray = new NullableIntArray()};
            o.NullableDoubleArray["zzz", 1] = 0;
            Assert.AreEqual(1, f(o, 0));
            Assert.AreEqual(1, o.NullableDoubleArray["zzz", 1]);
            o.NullableDoubleArray["zzz", 1] = 1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleArray["zzz", 1]);
            o.NullableDoubleArray["zzz", 1] = 2;
            Assert.AreEqual(16, f(o, 4));
            Assert.AreEqual(16, o.NullableDoubleArray["zzz", 1]);
            o.NullableDoubleArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, 2));
            Assert.AreEqual(1, o.NullableDoubleArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableDoubleArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableDoubleArray["zzz", 1]);
            o.NullableDoubleArray["zzz", 1] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleArray["zzz", 1]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableDoubleArray["zzz", 1]);
        }

        public class TestClassA
        {
            public NullableIntArray NullableDoubleArray { get; set; }
            public DoubleArray DoubleArray;
        }

        public class IntArray
        {
            public int this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    int[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new int[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new int[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            private readonly Dictionary<string, int[]> dict = new Dictionary<string, int[]>();
        }

        public class NullableIntArray
        {
            public double? this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    double?[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new double?[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new double?[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            private readonly Dictionary<string, double?[]> dict = new Dictionary<string, double?[]>();
        }

        public class UIntArray
        {
            public uint this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    uint[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new uint[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new uint[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            private readonly Dictionary<string, uint[]> dict = new Dictionary<string, uint[]>();
        }

        public class DoubleArray
        {
            public double this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    double[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new double[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new double[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            private readonly Dictionary<string, double[]> dict = new Dictionary<string, double[]>();
        }
    }
}