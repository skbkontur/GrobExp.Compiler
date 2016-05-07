using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.LeftShiftAssign
{
    [TestFixture]
    public class TestComplexProperty
    {
        [Test]
        public void Test1()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.LeftShiftAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), typeof(IntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new IntArray()};
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.IntArray["zzz", 1]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new IntArray()};
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.IntArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.LeftShiftAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntArray")), typeof(NullableIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new NullableIntArray()};
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.NullableIntArray["zzz", 1]);
            Assert.IsNull(f(null, 1));
            o.NullableIntArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new NullableIntArray()};
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.NullableIntArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);
        }

        [Test]
        public void Test3()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int?>> exp = Expression.Lambda<Func<TestClassA, int, int?>>(Expression.LeftShiftAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableIntArray")), typeof(NullableIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new NullableIntArray()};
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.NullableIntArray["zzz", 1]);
            Assert.IsNull(f(null, 1));
            o.NullableIntArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableIntArray = new NullableIntArray()};
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1234;
            Assert.AreEqual(-2468, f(o, 1));
            Assert.AreEqual(-2468, o.NullableIntArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableIntArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableIntArray["zzz", 1]);
        }

        [Test]
        public void Test4()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, uint>> exp = Expression.Lambda<Func<TestClassA, int, uint>>(Expression.LeftShiftAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("UIntArray")), typeof(UIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntArray = new UIntArray()};
            o.UIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.UIntArray["zzz", 1]);
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntArray = new UIntArray()};
            o.UIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.UIntArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void Test5()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, uint?>> exp = Expression.Lambda<Func<TestClassA, int, uint?>>(Expression.LeftShiftAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntArray")), typeof(NullableUIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntArray = new NullableUIntArray()};
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.NullableUIntArray["zzz", 1]);
            Assert.IsNull(f(null, 1));
            o.NullableUIntArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntArray["zzz", 1]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableUIntArray = new NullableUIntArray()};
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.NullableUIntArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableUIntArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntArray["zzz", 1]);
        }

        [Test]
        public void Test6()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, uint?>> exp = Expression.Lambda<Func<TestClassA, int?, uint?>>(Expression.LeftShiftAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntArray")), typeof(NullableUIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntArray = new NullableUIntArray()};
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.NullableUIntArray["zzz", 1]);
            Assert.IsNull(f(null, 1));
            o.NullableUIntArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntArray["zzz", 1]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntArray["zzz", 1]);

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {NullableUIntArray = new NullableUIntArray()};
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 10));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(1024, f(o, 10));
            Assert.AreEqual(1024, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1234;
            Assert.AreEqual(2468, f(o, 1));
            Assert.AreEqual(2468, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(16, f(o, 100));
            Assert.AreEqual(16, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.AreEqual(2147483648, f(o, 31));
            Assert.AreEqual(2147483648, o.NullableUIntArray["zzz", 1]);
            Assert.Throws<NullReferenceException>(() => f(null, 1));
            o.NullableUIntArray["zzz", 1] = null;
            Assert.IsNull(f(o, 2));
            Assert.IsNull(o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntArray["zzz", 1]);
            Assert.IsNull(f(o, null));
            Assert.IsNull(o.NullableUIntArray["zzz", 1]);
        }

        public class TestClassA
        {
            public IntArray IntArray { get; set; }
            public UIntArray UIntArray { get; set; }
            public NullableIntArray NullableIntArray;
            public NullableUIntArray NullableUIntArray;
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

        public class NullableUIntArray
        {
            public uint? this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    uint?[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new uint?[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new uint?[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            private readonly Dictionary<string, uint?[]> dict = new Dictionary<string, uint?[]>();
        }
    }
}