using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.PostIncrementAssign
{
    [TestFixture]
    public class TestComplexProperty
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(Expression.PostIncrementAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), typeof(IntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)})), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new IntArray()};
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = -1;
            Assert.AreEqual(-1, f(o));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = int.MaxValue;
            Assert.AreEqual(int.MaxValue, f(o));
            Assert.AreEqual(int.MinValue, o.IntArray["zzz", 1]);
            Assert.AreEqual(0, f(null));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new IntArray()};
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = -1;
            Assert.AreEqual(-1, f(o));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = int.MaxValue;
            Assert.AreEqual(int.MaxValue, f(o));
            Assert.AreEqual(int.MinValue, o.IntArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, int?>> exp = Expression.Lambda<Func<TestClassA, int?>>(Expression.PostIncrementAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), typeof(NullableIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)})), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new NullableIntArray()};
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1;
            Assert.AreEqual(-1, f(o));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = int.MaxValue;
            Assert.AreEqual(int.MaxValue, f(o));
            Assert.AreEqual(int.MinValue, o.NullableIntArray["zzz", 1]);
            Assert.IsNull(f(null));
            o.NullableIntArray["zzz", 1] = null;
            Assert.IsNull(f(o));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new NullableIntArray()};
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1;
            Assert.AreEqual(-1, f(o));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = int.MaxValue;
            Assert.AreEqual(int.MaxValue, f(o));
            Assert.AreEqual(int.MinValue, o.NullableIntArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null));
            o.NullableIntArray["zzz", 1] = null;
            Assert.IsNull(f(o));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);
        }

        [Test]
        public void TestDouble()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            Expression<Func<TestClassA, double>> exp = Expression.Lambda<Func<TestClassA, double>>(Expression.PostIncrementAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("DoubleArray")), typeof(DoubleArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)})), a);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {DoubleArray = new DoubleArray()};
            o.DoubleArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(1, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = -1;
            Assert.AreEqual(-1, f(o));
            Assert.AreEqual(0, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = -0.5;
            Assert.AreEqual(-0.5, f(o));
            Assert.AreEqual(0.5, o.DoubleArray["zzz", 1]);
            Assert.AreEqual(0, f(null));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {DoubleArray = new DoubleArray()};
            o.DoubleArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(1, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = -1;
            Assert.AreEqual(-1, f(o));
            Assert.AreEqual(0, o.DoubleArray["zzz", 1]);
            o.DoubleArray["zzz", 1] = -0.5;
            Assert.AreEqual(-0.5, f(o));
            Assert.AreEqual(0.5, o.DoubleArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null));
        }

        public class TestClassA
        {
            public IntArray IntArray { get; set; }
            public NullableIntArray NullableIntArray { get; set; }
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
            public int? this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    int?[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new int?[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new int?[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            private readonly Dictionary<string, int?[]> dict = new Dictionary<string, int?[]>();
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