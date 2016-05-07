using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.SubtractAssign
{
    [TestFixture]
    public class TestComplexProperty
    {
        [Test]
        public void TestInt()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.SubtractAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), typeof(IntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new IntArray()};
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, -2));
            Assert.AreEqual(1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 - -2000000000, f(o, -2000000000));
                Assert.AreEqual(2000000000 - -2000000000, o.IntArray["zzz", 1]);
            }
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new IntArray()};
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, -2));
            Assert.AreEqual(1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 - -2000000000, f(o, -2000000000));
                Assert.AreEqual(2000000000 - -2000000000, o.IntArray["zzz", 1]);
            }
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestNullable()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.SubtractAssign(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), typeof(NullableIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new NullableIntArray()};
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, -2));
            Assert.AreEqual(1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 - -2000000000, f(o, -2000000000));
                Assert.AreEqual(2000000000 - -2000000000, o.NullableIntArray["zzz", 1]);
            }
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
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, -2));
            Assert.AreEqual(1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 2000000000;
            unchecked
            {
                Assert.AreEqual(2000000000 - -2000000000, f(o, -2000000000));
                Assert.AreEqual(2000000000 - -2000000000, o.NullableIntArray["zzz", 1]);
            }
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
        public void TestCheckedSigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int), "b");
            Expression<Func<TestClassA, int, int>> exp = Expression.Lambda<Func<TestClassA, int, int>>(Expression.SubtractAssignChecked(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("IntArray")), typeof(IntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {IntArray = new IntArray()};
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, -2));
            Assert.AreEqual(1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, -2000000000));
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {IntArray = new IntArray()};
            o.IntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 1;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, -2));
            Assert.AreEqual(1, o.IntArray["zzz", 1]);
            o.IntArray["zzz", 1] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, -2000000000));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestCheckedNullableSigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(int?), "b");
            Expression<Func<TestClassA, int?, int?>> exp = Expression.Lambda<Func<TestClassA, int?, int?>>(Expression.SubtractAssignChecked(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetProperty("NullableIntArray")), typeof(NullableIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableIntArray = new NullableIntArray()};
            o.NullableIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, -2));
            Assert.AreEqual(1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, -2000000000));
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
            o.NullableIntArray["zzz", 1] = 1;
            Assert.AreEqual(-1, f(o, 2));
            Assert.AreEqual(-1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = -1;
            Assert.AreEqual(1, f(o, -2));
            Assert.AreEqual(1, o.NullableIntArray["zzz", 1]);
            o.NullableIntArray["zzz", 1] = 2000000000;
            Assert.Throws<OverflowException>(() => f(o, -2000000000));
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
        public void TestCheckedUnsigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint), "b");
            Expression<Func<TestClassA, uint, uint>> exp = Expression.Lambda<Func<TestClassA, uint, uint>>(Expression.SubtractAssignChecked(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("UIntArray")), typeof(UIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {UIntArray = new UIntArray()};
            o.UIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 4000000000;
            Assert.AreEqual(3000000000, f(o, 1000000000));
            Assert.AreEqual(3000000000, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1;
            Assert.Throws<OverflowException>(() => f(o, 2));
            Assert.AreEqual(0, f(null, 1));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o = new TestClassA {UIntArray = new UIntArray()};
            o.UIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 4000000000;
            Assert.AreEqual(3000000000, f(o, 1000000000));
            Assert.AreEqual(3000000000, o.UIntArray["zzz", 1]);
            o.UIntArray["zzz", 1] = 1;
            Assert.Throws<OverflowException>(() => f(o, 2));
            Assert.Throws<NullReferenceException>(() => f(null, 1));
        }

        [Test]
        public void TestCheckedNullableUnsigned()
        {
            ParameterExpression a = Expression.Parameter(typeof(TestClassA), "a");
            ParameterExpression b = Expression.Parameter(typeof(uint?), "b");
            Expression<Func<TestClassA, uint?, uint?>> exp = Expression.Lambda<Func<TestClassA, uint?, uint?>>(Expression.SubtractAssignChecked(Expression.MakeIndex(Expression.MakeMemberAccess(a, typeof(TestClassA).GetField("NullableUIntArray")), typeof(NullableUIntArray).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), b), a, b);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences);
            var o = new TestClassA {NullableUIntArray = new NullableUIntArray()};
            o.NullableUIntArray["zzz", 1] = 0;
            Assert.AreEqual(0, f(o, 0));
            Assert.AreEqual(0, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 4000000000;
            Assert.AreEqual(3000000000, f(o, 1000000000));
            Assert.AreEqual(3000000000, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.Throws<OverflowException>(() => f(o, 2));
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
            o.NullableUIntArray["zzz", 1] = 4000000000;
            Assert.AreEqual(3000000000, f(o, 1000000000));
            Assert.AreEqual(3000000000, o.NullableUIntArray["zzz", 1]);
            o.NullableUIntArray["zzz", 1] = 1;
            Assert.Throws<OverflowException>(() => f(o, 2));
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
            public NullableIntArray NullableIntArray { get; set; }
            public UIntArray UIntArray;
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