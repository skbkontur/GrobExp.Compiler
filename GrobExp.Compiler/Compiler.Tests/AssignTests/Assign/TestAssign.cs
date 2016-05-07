using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.AssignTests.Assign
{
    [TestFixture]
    public class TestAssign
    {
        [Test]
        public void TestAssign1()
        {
            var parameter = Expression.Parameter(typeof(int));
            var assign = Expression.Assign(parameter, Expression.Constant(-1));
            var exp = Expression.Lambda<Func<int, int>>(assign, parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-1, f(1));
        }

        [Test]
        public void TestAssign2()
        {
            var parameter = Expression.Parameter(typeof(int));
            var variable = Expression.Parameter(typeof(int));
            var assign = Expression.Assign(variable, parameter);
            var body = Expression.Block(typeof(int), new[] {variable}, assign);
            var exp = Expression.Lambda<Func<int, int>>(body, parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(1));
        }

        [Test]
        public void TestAssign3()
        {
            Expression<Func<TestClassA, bool>> path = a => a.Bool;
            Expression<Func<TestClassA, bool>> condition = a => a.X > 0;
            Expression body = Expression.Assign(path.Body, new ParameterReplacer(condition.Parameters[0], path.Parameters[0]).Visit(condition.Body));
            Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(body, path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            f(o);
            Assert.AreEqual(false, o.Bool);
            o.X = 1;
            f(o);
            Assert.AreEqual(true, o.Bool);
        }

        [Test]
        public void TestAssign4()
        {
            Expression<Func<TestClassA, int>> path1 = a => a.Y;
            Expression<Func<TestClassA, int>> path2 = a => a.B.Y;
            Expression body = Expression.Assign(path1.Body, new ParameterReplacer(path2.Parameters[0], path1.Parameters[0]).Visit(path2.Body));
            Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            f(o);
            Assert.AreEqual(0, o.Y);
            o.B = new TestClassB();
            f(o);
            Assert.AreEqual(0, o.Y);
            o.B.Y = 12;
            f(o);
            Assert.AreEqual(12, o.Y);
            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            o.B.Y = 123;
            f(o);
            Assert.AreEqual(123, o.Y);
        }

        [Test]
        public void TestAssignStaticField1()
        {
            Expression<Func<int>> path = () => x;
            ParameterExpression parameter = Expression.Parameter(typeof(int));
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(Expression.Assign(path.Body, parameter), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(-123, f(-123));
            Assert.AreEqual(-123, x);
        }

        [Test]
        public void TestAssignStaticField2()
        {
            Expression<Func<string>> path = () => S;
            ParameterExpression parameter = Expression.Parameter(typeof(string));
            Expression<Func<string, string>> exp = Expression.Lambda<Func<string, string>>(Expression.Assign(path.Body, parameter), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("zzz", f("zzz"));
            Assert.AreEqual("zzz", S);
        }

        [Test]
        public void TestAssignStaticField3()
        {
            Expression<Func<bool>> path = () => b;
            Expression<Func<TestClassA, bool>> condition = a => a.X > 0;
            Expression<Func<TestClassA, bool>> exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.Assign(path.Body, condition.Body), condition.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(false, f(null));
            Assert.AreEqual(false, b);
            Assert.AreEqual(false, f(new TestClassA()));
            Assert.AreEqual(false, b);
            Assert.AreEqual(false, f(new TestClassA {X = -1}));
            Assert.AreEqual(false, b);
            Assert.AreEqual(true, f(new TestClassA {X = 1}));
            Assert.AreEqual(true, b);
        }

        [Test]
        public void TestAssignStaticField4()
        {
            Expression<Func<int>> path = () => x;
            Expression<Func<TestClassA, int>> condition = a => a.Y;
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(Expression.Assign(path.Body, condition.Body), condition.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(null));
            Assert.AreEqual(0, x);
            Assert.AreEqual(0, f(new TestClassA()));
            Assert.AreEqual(0, x);
            Assert.AreEqual(-1, f(new TestClassA {Y = -1}));
            Assert.AreEqual(-1, x);
        }

        [Test]
        public void TestAssignWithExtend1()
        {
            Expression<Func<TestClassA, string>> exp = a => a.B.S;
            Expression<Func<TestClassA, string>> exp2 = Expression.Lambda<Func<TestClassA, string>>(Expression.Assign(exp.Body, Expression.Constant("zzz")), exp.Parameters);
            var f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            var o = new TestClassA();
            Assert.AreEqual("zzz", f(o));
            Assert.IsNotNull(o.B);
            Assert.AreEqual("zzz", o.B.S);
        }

        [Test]
        public void TestAssignWithExtend1Struct()
        {
            Expression<Func<TestClassA, string>> exp = a => a.structA.b.S;
            Expression<Func<TestClassA, string>> exp2 = Expression.Lambda<Func<TestClassA, string>>(Expression.Assign(exp.Body, Expression.Constant("zzz")), exp.Parameters);
            var f = LambdaCompiler.Compile(exp2, CompilerOptions.All);
            var o = new TestClassA();
            Assert.AreEqual("zzz", f(o));
            Assert.AreEqual("zzz", o.structA.b.S);
        }

        [Test]
        public void TestAssignWithExtend2()
        {
            Expression<Func<TestClassA, string>> path1 = a => a.B.S;
            Expression<Func<TestClassA, string>> path2 = a => a.B.C.S;
            Expression body = Expression.Block(typeof(string), Expression.Assign(path1.Body, Expression.Constant("zzz")), Expression.Assign(new ParameterReplacer(path2.Parameters[0], path1.Parameters[0]).Visit(path2.Body), Expression.Constant("qxx")));
            Expression<Func<TestClassA, string>> exp = Expression.Lambda<Func<TestClassA, string>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            Assert.AreEqual("qxx", f(o));
            Assert.IsNotNull(o.B);
            Assert.AreEqual("zzz", o.B.S);
            Assert.IsNotNull(o.B.C);
            Assert.AreEqual("qxx", o.B.C.S);
        }

        [Test]
        public void TestAssignWithExtend3()
        {
            Expression<Func<TestClassA, string>> path1 = a => a.B.S;
            Expression<Func<TestClassA, string>> path2 = a => a.B.C.S;
            Expression body = Expression.Block(typeof(void), Expression.Assign(path1.Body, Expression.Constant("zzz")), Expression.Assign(new ParameterReplacer(path2.Parameters[0], path1.Parameters[0]).Visit(path2.Body), Expression.Constant("qxx")));
            Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(body, path1.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            f(o);
            Assert.IsNotNull(o.B);
            Assert.AreEqual("zzz", o.B.S);
            Assert.IsNotNull(o.B.C);
            Assert.AreEqual("qxx", o.B.C.S);
        }

        [Test]
        public void TestAssignWithExtend4()
        {
            Expression<Func<TestClassA, string>> path = a => a.B.S;
            ParameterExpression parameter = Expression.Parameter(typeof(TestClassA));
            Expression body = Expression.Block(typeof(string), new[] {path.Parameters[0]}, Expression.Assign(path.Parameters[0], parameter), Expression.Assign(path.Body, Expression.Constant("zzz")));
            Expression<Func<TestClassA, string>> exp = Expression.Lambda<Func<TestClassA, string>>(body, parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            Assert.AreEqual("zzz", f(o));
            Assert.IsNotNull(o.B);
            Assert.AreEqual("zzz", o.B.S);
        }

        [Test]
        public void TestAssignToComplexProperty1()
        {
            var parameter = Expression.Parameter(typeof(TestClassA));
            var exp = Expression.Lambda<Func<TestClassA, string>>(Expression.Assign(Expression.MakeIndex(parameter, typeof(TestClassA).GetProperty("Item"), new[] {Expression.Constant("zzz"), Expression.Constant(1)}), Expression.Constant("qzz")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA();
            a["zzz", 1] = "qxx";
            Assert.AreEqual("qzz", f(a));
            Assert.AreEqual("qzz", a["zzz", 1]);
        }

        [Test]
        public void TestAssignToDictionary1()
        {
            var parameter = Expression.Parameter(typeof(TestClassA));
            var exp = Expression.Lambda<Func<TestClassA, string>>(Expression.Assign(Expression.Property(Expression.MakeIndex(Expression.Property(parameter, "Dict"), typeof(Dictionary<string, TestClassB>).GetProperty("Item"), new[] {Expression.Constant("zzz")}), "S"), Expression.Constant("2")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA{Dict = new Dictionary<string, TestClassB>{{"zzz", new TestClassB{S = "1"}}}};
            Assert.AreEqual("2", f(a));
            Assert.AreEqual("2", a.Dict["zzz"].S);
            a = new TestClassA();
            Assert.AreEqual("2", f(a));
            Assert.AreEqual("2", a.Dict["zzz"].S);
        }

        [Test]
        public void TestAssignToDictionary2()
        {
            var parameter = Expression.Parameter(typeof(TestClassA));
            var exp = Expression.Lambda<Func<TestClassA, string>>(Expression.Assign(Expression.Property(Expression.Call(Expression.Property(parameter, "Dict"), typeof(Dictionary<string, TestClassB>).GetProperty("Item").GetGetMethod(), new[] {Expression.Constant("zzz")}), "S"), Expression.Constant("2")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var a = new TestClassA{Dict = new Dictionary<string, TestClassB>{{"zzz", new TestClassB{S = "1"}}}};
            Assert.AreEqual("2", f(a));
            Assert.AreEqual("2", a.Dict["zzz"].S);
            a = new TestClassA();
            Assert.AreEqual("2", f(a));
            Assert.AreEqual("2", a.Dict["zzz"].S);

        }

        [Test]
        public void TestAssignToList()
        {
            //LambdaCompiler.DebugOutputDirectory = @"c:\temp";
            var parameter = Expression.Parameter(typeof(TestClassA));
            var exp = Expression.Lambda<Func<TestClassA, string>>(Expression.Assign(Expression.MakeIndex(Expression.Property(parameter, "List"), typeof(List<string>).GetProperty("Item"), new[] { Expression.Constant(1) }), Expression.Constant("zzz")), parameter);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA();
            Assert.AreEqual("zzz", f(o));
            Assert.IsNotNull(o.List);
            Assert.AreEqual(2, o.List.Count);
            Assert.AreEqual("zzz", o.List[1]);
        }

        public static string S { get; set; }

        public static int x;
        public static bool b;

        public class TestClassA
        {
            public int F(bool b)
            {
                return b ? 1 : 0;
            }

            public Dictionary<string, TestClassB> Dict { get; set; }
            private readonly List<string> _list = new List<string>();
            public List<string> List { get { return _list; } }

            public string S { get; set; }
            public TestClassA A { get; set; }
            public TestClassB B { get; set; }
            public TestClassB[] ArrayB { get; set; }

            public string this[string key, int index]
            {
                get { return dict[key][index]; }
                set
                {
                    string[] array;
                    if(!dict.TryGetValue(key, out array))
                        dict.Add(key, array = new string[0]);
                    if(array.Length <= index)
                    {
                        var newArray = new string[index + 1];
                        array.CopyTo(newArray, 0);
                        array = dict[key] = newArray;
                    }
                    array[index] = value;
                }
            }

            public int? X;
            public Guid Guid = Guid.Empty;
            public Guid? NullableGuid;
            public bool? NullableBool;
            public int Y;
            public bool Bool;
            public TestStructA structA;

            private readonly Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
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