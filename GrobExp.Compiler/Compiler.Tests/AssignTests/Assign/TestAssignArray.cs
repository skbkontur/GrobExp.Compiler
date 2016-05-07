using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.Assign
{
    [TestFixture]
    public class TestAssignArray
    {
        [Test]
        public void TestAssignArrayWithExtend1()
        {
            Expression<Func<TestClassA, string>> path1 = a => a.ArrayB[1].S;
            Expression body = Expression.Assign(path1.Body, Expression.Constant("zzz"));
            Expression<Func<TestClassA, string>> exp = Expression.Lambda<Func<TestClassA, string>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {ArrayB = new[] {new TestClassB {S = "qxx"},}};
            Assert.AreEqual("zzz", f(o));
            Assert.IsNotNull(o.ArrayB);
            Assert.AreEqual(2, o.ArrayB.Length);
            Assert.AreEqual("zzz", o.ArrayB[1].S);
            Assert.AreEqual("qxx", o.ArrayB[0].S);
        }

        [Test]
        public void TestAssignArrayWithExtend2()
        {
            Expression<Func<TestClassA, string>> path1 = a => a.ArrayB[1].C.ArrayD[2].ArrayE[3].S;
            Expression body = Expression.Assign(path1.Body, Expression.Constant("zzz"));
            Expression<Func<TestClassA, string>> exp = Expression.Lambda<Func<TestClassA, string>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            Assert.AreEqual("zzz", f(o));
            Assert.IsNotNull(o.ArrayB);
            Assert.AreEqual(2, o.ArrayB.Length);
            Assert.IsNotNull(o.ArrayB[1].C);
            Assert.IsNotNull(o.ArrayB[1].C.ArrayD);
            Assert.AreEqual(3, o.ArrayB[1].C.ArrayD.Length);
            Assert.IsNotNull(o.ArrayB[1].C.ArrayD[2].ArrayE);
            Assert.AreEqual(4, o.ArrayB[1].C.ArrayD[2].ArrayE.Length);
            Assert.AreEqual("zzz", o.ArrayB[1].C.ArrayD[2].ArrayE[3].S);
        }

        [Test]
        public void TestAssignArrayWithExtend3()
        {
            Expression<Func<TestClassA, int[]>> path1 = a => a.IntArray;
            Expression body = Expression.Assign(Expression.ArrayAccess(path1.Body, Expression.Constant(1)), Expression.Constant(-123));
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {IntArray = new[] {12}};
            Assert.AreEqual(-123, f(o));
            Assert.IsNotNull(o.IntArray);
            Assert.AreEqual(2, o.IntArray.Length);
            Assert.AreEqual(12, o.IntArray[0]);
            Assert.AreEqual(-123, o.IntArray[1]);
        }

        [Test]
        public void TestAssignArrayWithExtend4()
        {
            var parameter = Expression.Parameter(typeof(int[]));
            var exp = Expression.Lambda<Func<int[], int>>(Expression.Block(typeof(int), Expression.Assign(Expression.ArrayAccess(parameter, Expression.Constant(1)), Expression.Constant(-123)), Expression.ArrayIndex(parameter, Expression.Constant(1))), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-123, f(new int[0]));
        }

        [Test]
        public void TestAssignArrayWithExtend5()
        {
            Expression<Func<int[]>> path = () => intArray;
            var exp = Expression.Lambda<Func<int>>(Expression.Block(typeof(int), Expression.Assign(Expression.ArrayAccess(path.Body, Expression.Constant(1)), Expression.Constant(-123)), Expression.ArrayIndex(path.Body, Expression.Constant(1))));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-123, f());
        }

        [Test]
        public void TestAssignArrayWithExtend6()
        {
            Expression<Func<TestClassA, int[][]>> path1 = a => a.DoubleIntArray;
            Expression body = Expression.Assign(Expression.ArrayAccess(Expression.ArrayAccess(path1.Body, Expression.Constant(1)), Expression.Constant(1)), Expression.Constant(-123));
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {DoubleIntArray = new[] {new[] {12}}};
            Assert.AreEqual(-123, f(o));
            Assert.IsNotNull(o.DoubleIntArray);
            Assert.AreEqual(2, o.DoubleIntArray.Length);
            Assert.IsNotNull(o.DoubleIntArray[0]);
            Assert.AreEqual(1, o.DoubleIntArray[0].Length);
            Assert.AreEqual(12, o.DoubleIntArray[0][0]);
            Assert.IsNotNull(o.DoubleIntArray[1]);
            Assert.AreEqual(2, o.DoubleIntArray[1].Length);
            Assert.AreEqual(-123, o.DoubleIntArray[1][1]);
        }

        [Test]
        public void TestAssignArrayWithExtend7()
        {
            Expression<Func<TestClassA, int[][]>> path1 = a => a.DoubleIntArray;
            Expression body = Expression.Assign(Expression.ArrayAccess(Expression.ArrayIndex(path1.Body, Expression.Constant(1)), Expression.Constant(1)), Expression.Constant(-123));
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {DoubleIntArray = new[] {new[] {12}}};
            Assert.AreEqual(-123, f(o));
            Assert.IsNotNull(o.DoubleIntArray);
            Assert.AreEqual(2, o.DoubleIntArray.Length);
            Assert.IsNotNull(o.DoubleIntArray[0]);
            Assert.AreEqual(1, o.DoubleIntArray[0].Length);
            Assert.AreEqual(12, o.DoubleIntArray[0][0]);
            Assert.IsNotNull(o.DoubleIntArray[1]);
            Assert.AreEqual(2, o.DoubleIntArray[1].Length);
            Assert.AreEqual(-123, o.DoubleIntArray[1][1]);
        }

        [Test]
        public void TestAssignArrayWithExtend8()
        {
            Expression<Func<TestClassA, string[]>> path = a => a.ArrayB[0].C.ArrayD[0].StringArray;
            Expression body = Expression.Assign(Expression.ArrayAccess(path.Body, Expression.Constant(1)), Expression.Constant("zzz"));
            Expression<Func<TestClassA, string>> exp = Expression.Lambda<Func<TestClassA, string>>(body, path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            Assert.AreEqual("zzz", f(o));
            Assert.IsNotNull(o.ArrayB);
            Assert.AreEqual(1, o.ArrayB.Length);
            Assert.IsNotNull(o.ArrayB[0].C);
            Assert.IsNotNull(o.ArrayB[0].C.ArrayD);
            Assert.AreEqual(1, o.ArrayB[0].C.ArrayD.Length);
            Assert.IsNotNull(o.ArrayB[0].C.ArrayD[0].StringArray);
            Assert.AreEqual(2, o.ArrayB[0].C.ArrayD[0].StringArray.Length);
            Assert.IsNull(o.ArrayB[0].C.ArrayD[0].StringArray[0]);
            Assert.AreEqual("zzz", o.ArrayB[0].C.ArrayD[0].StringArray[1]);
        }

        [Test]
        public void TestAssignArrayWithExtend9()
        {
            Expression<Func<TestClassA, int>> path1 = a => a.TwoDimensionalArray[0, 0][0].Y;
            Expression body = Expression.Assign(path1.Body, Expression.Constant(-123));
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.CheckNullReferences | CompilerOptions.ExtendOnAssign);
            var o = new TestClassA {TwoDimensionalArray = new TestClassB[1,1][]};
            o.TwoDimensionalArray[0, 0] = new TestClassB[1];
            Assert.AreEqual(-123, f(o));
            Assert.IsNotNull(o.TwoDimensionalArray[0, 0][0]);
            Assert.AreEqual(-123, o.TwoDimensionalArray[0, 0][0].Y);
        }

        [Test]
        public void TestAssignArrayWithExtend10()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int[]));
            ParameterExpression variable = Expression.Parameter(typeof(int[]));
            Expression body = Expression.Block(typeof(int), new[] {variable}, Expression.Assign(variable, parameter), Expression.Assign(Expression.ArrayAccess(variable, Expression.Constant(1)), Expression.Constant(-123)));
            var exp = Expression.Lambda<Func<int[], int>>(body, parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-123, f(new int[0]));
        }

        [Test]
        public void TestAssignArrayWithExtend11()
        {
            Expression<Func<TestClassA, bool[]>> path = a => a.BoolArray;
            Expression<Func<TestClassA, bool>> condition = a => a.X > 0;
            Expression body = Expression.Assign(Expression.ArrayAccess(path.Body, Expression.Constant(1)), new ParameterReplacer(condition.Parameters[0], path.Parameters[0]).Visit(condition.Body));
            Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(body, path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            f(o);
            Assert.IsNotNull(o.BoolArray);
            Assert.AreEqual(2, o.BoolArray.Length);
            Assert.AreEqual(false, o.BoolArray[1]);
            o.X = 1;
            f(o);
            Assert.AreEqual(true, o.BoolArray[1]);
        }

        [Test]
        public void TestAssignArrayWithExtend12()
        {
            Expression<Func<TestClassA, int[]>> path1 = a => a.IntArray;
            Expression<Func<TestClassA, int>> path2 = a => a.Y;
            Expression body = Expression.Assign(Expression.ArrayAccess(path1.Body, Expression.Constant(1)), new ParameterReplacer(path2.Parameters[0], path1.Parameters[0]).Visit(path2.Body));
            Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            f(o);
            Assert.IsNotNull(o.IntArray);
            Assert.AreEqual(2, o.IntArray.Length);
            Assert.AreEqual(0, o.IntArray[1]);
            o.Y = 1;
            f(o);
            Assert.AreEqual(1, o.IntArray[1]);
            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o.Y = 123;
            f(o);
            Assert.AreEqual(123, o.IntArray[1]);
        }

        [Test]
        public void TestAssignArrayWithExtend13()
        {
            Expression<Func<TestClassA, DateTime[]>> path1 = a => a.DateTimeArray;
            Expression<Func<DateTime>> path2 = () => MyBirthDate;
            Expression body = Expression.Assign(Expression.ArrayAccess(path1.Body, Expression.Constant(1)), path2.Body);
            Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            f(o);
            Assert.IsNotNull(o.DateTimeArray);
            Assert.AreEqual(2, o.DateTimeArray.Length);
            Assert.AreEqual(MyBirthDate, o.DateTimeArray[1]);
            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            f(o);
            Assert.AreEqual(MyBirthDate, o.DateTimeArray[1]);
        }

        [Test]
        public void TestAssignToMultiDimensionalArray1()
        {
            Expression<Func<TestClassA, string[,]>> path = a => a.StringArray;
            Expression body = Expression.Assign(Expression.ArrayAccess(path.Body, Expression.Constant(0), Expression.Constant(0)), Expression.Constant("zzz"));
            Expression<Func<TestClassA, string>> exp = Expression.Lambda<Func<TestClassA, string>>(body, path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {StringArray = new string[1,1]};
            o.StringArray[0, 0] = "qxx";
            Assert.AreEqual("zzz", f(o));
            Assert.AreEqual("zzz", o.StringArray[0, 0]);
        }

        [Test]
        public void TestAssignToMultiDimensionalArray2()
        {
            Expression<Func<TestClassA, bool[,]>> path = a => a.B.BoolArray;
            Expression<Func<TestClassA, bool>> condition = a => a.X > 0;
            Expression body = Expression.Assign(Expression.ArrayAccess(path.Body, Expression.Constant(0), Expression.Constant(0)), new ParameterReplacer(condition.Parameters[0], path.Parameters[0]).Visit(condition.Body));
            Expression<Func<TestClassA, bool>> exp = Expression.Lambda<Func<TestClassA, bool>>(body, path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {B = new TestClassB {BoolArray = new bool[1,1]}};
            o.B.BoolArray[0, 0] = true;
            Assert.AreEqual(false, f(o));
            Assert.AreEqual(false, o.B.BoolArray[0, 0]);
            o.X = 1;
            Assert.AreEqual(true, f(o));
            Assert.AreEqual(true, o.B.BoolArray[0, 0]);
        }

        [Test]
        public void TestAssignToMultiDimensionalArray3()
        {
            Expression<Func<TestClassA, int[,]>> path1 = a => a.B.IntArray;
            Expression<Func<TestClassA, int>> path2 = a => a.B.C.D.Y;
            Expression body = Expression.Assign(Expression.ArrayAccess(path1.Body, Expression.Constant(0), Expression.Constant(0)), new ParameterReplacer(path2.Parameters[0], path1.Parameters[0]).Visit(path2.Body));
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {B = new TestClassB {IntArray = new int[1,1]}};
            o.B.IntArray[0, 0] = 123;
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.B.IntArray[0, 0]);
            o.B.C = new TestClassC();
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.B.IntArray[0, 0]);
            o.B.C.D = new TestClassD();
            Assert.AreEqual(0, f(o));
            Assert.AreEqual(0, o.B.IntArray[0, 0]);
            o.B.C.D.Y = -123;
            Assert.AreEqual(-123, f(o));
            Assert.AreEqual(-123, o.B.IntArray[0, 0]);
            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o.B.C.D.Y = 123;
            Assert.AreEqual(123, f(o));
            Assert.AreEqual(123, o.B.IntArray[0, 0]);
        }

        [Test]
        public void TestAssignToStaticMultidimensionalArray()
        {
            Expression<Func<int[,]>> path = () => TwoDimensionalIntArray;
            Expression body = Expression.Assign(Expression.ArrayAccess(path.Body, Expression.Constant(1), Expression.Constant(2)), Expression.Constant(-123));
            Expression<Func<int>> exp = Expression.Lambda<Func<int>>(body);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            TwoDimensionalIntArray = new int[3,3];
            TwoDimensionalIntArray[1, 2] = 82736;
            Assert.AreEqual(-123, f());
            Assert.AreEqual(-123, TwoDimensionalIntArray[1, 2]);
        }

        public static int[,] TwoDimensionalIntArray { get; set; }

        public static DateTime MyBirthDate { get { return new DateTime(1986, 2, 16); } }
        public static string S { get; set; }

        public static int[] intArray;
        public static int x;
        public static bool b;

        public class TestClassA
        {
            public int F(bool b)
            {
                return b ? 1 : 0;
            }

            public string S { get; set; }
            public TestClassA A { get; set; }
            public TestClassB B { get; set; }
            public TestClassB[] ArrayB { get; set; }
            public int[] IntArray { get; set; }
            public int[][] DoubleIntArray { get; set; }
            public bool[] BoolArray { get; set; }
            public TestClassB[,][] TwoDimensionalArray { get; set; }
            public DateTime[] DateTimeArray { get; set; }
            public string[,] StringArray { get; set; }

            public int? X;
            public Guid Guid = Guid.Empty;
            public Guid? NullableGuid;
            public bool? NullableBool;
            public int Y;
            public bool Bool;
            public TestStructA structA;
        }

        public class TestClassB
        {
            public int? F2(int? x)
            {
                return x;
            }

            public int? F( /*Qzz*/ int a, int b)
            {
                return b;
            }

            public string S { get; set; }
            public bool[,] BoolArray { get; set; }
            public int[,] IntArray { get; set; }

            public TestClassC C { get; set; }
            public int? X;
            public int Y;
        }

        public class TestClassC
        {
            public string S { get; set; }

            public TestClassD D { get; set; }

            public TestClassD[] ArrayD { get; set; }
        }

        public class TestClassD
        {
            public TestClassE E { get; set; }
            public TestClassE[] ArrayE { get; set; }
            public string Z { get; set; }
            public string[] StringArray { get; set; }

            public int? X { get; set; }
            public int Y { get; set; }

            public readonly string S;
        }

        public class TestClassE
        {
            public string S { get; set; }
            public int X { get; set; }
        }

        public struct TestStructA
        {
            public string S { get; set; }
            public int? X { get; set; }
            public int Y { get; set; }
            public TestStructB b;
        }

        public struct TestStructB
        {
            public string S { get; set; }
        }

        public struct Qzz
        {
            public long X;
        }
    }
}