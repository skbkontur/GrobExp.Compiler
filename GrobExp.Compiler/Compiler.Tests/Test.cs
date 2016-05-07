using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class Test // todo растащить на куски
    {
        public class Qzz
        {
            public int? X { get; set; }
        }

        [Test]
        public void TestNullable()
        {
            Expression<Func<Qzz, int>> exp = x => x.X.Value;
            var func = LambdaCompiler.Compile(exp, CompilerOptions.All);
            func(null);
        }

        [Test]
        public void TestRefParameter()
        {
            Expression<Action<TestClassA>> exp = a => Array.Resize(ref a.IntArray2, 5);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {IntArray2 = new int[] {1, 2, 3}};
            f(o);
            Assert.AreEqual(5, o.IntArray2.Length);
            Assert.AreEqual(1, o.IntArray2[0]);
            Assert.AreEqual(2, o.IntArray2[1]);
            Assert.AreEqual(3, o.IntArray2[2]);
        }

        public static int Zzz(int x, out int y)
        {
            y = x + 1;
            return x;
        }

        [Test]
        public void TestOutParameter()
        {
            int y = 0;
            Expression<Func<int, int>> exp = x => Zzz(x, out y);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(5, f(5));
            Assert.AreEqual(6, y);
        }

        [Test, Ignore]
        public void TestPinning1()
        {
            Expression<Func<TestClassA, bool>> path = a => a.StructAArray[0].S == GetString();
            Expression<Func<TestClassA, bool>> exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.Block(typeof(bool), Expression.Assign(((BinaryExpression)path.Body).Left, Expression.Call(getStringMethod)), path.Body), path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var collectingThread = new Thread(Collect);
            collectingThread.Start();
            for(int i = 0; i < 10; ++i)
            {
                var generatingTrashThread = new Thread(GenerateTrash);
                generatingTrashThread.Start();
            }
            for(int i = 0; i < 100000; ++i)
            {
                var o = new TestClassA {ArrayB = new TestClassB[100], IntArray = new int[100]};
                Assert.IsTrue(f(o));
            }
        }

        [Test, Ignore]
        public void TestPinning2()
        {
            Expression<Func<TestClassA, bool>> path = a => a.StringArray[1] == GetString();
            var arrayIndex = (BinaryExpression)((BinaryExpression)path.Body).Left;
            Expression arrayAccess = Expression.ArrayAccess(arrayIndex.Left, arrayIndex.Right);
            Expression<Func<TestClassA, bool>> exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.Block(typeof(bool), Expression.Assign(arrayAccess, Expression.Call(getStringMethod)), path.Body), path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var collectingThread = new Thread(Collect);
            collectingThread.Start();
            for(int i = 0; i < 10; ++i)
            {
                var generatingTrashThread = new Thread(GenerateTrash);
                generatingTrashThread.Start();
            }
            for(int i = 0; i < 100000; ++i)
            {
                var o = new TestClassA {ArrayB = new TestClassB[100], IntArray = new int[100], StringArray = new string[100]};
                Assert.IsTrue(f(o));
            }
        }

        [Test]
        public void TestConstsAreNotFreedWhileRunning()
        {
            var guid = new Guid("2e224f5f-e392-4753-a19a-4304f226b965");
            ParameterExpression parameter = Expression.Parameter(typeof(Guid));
            Expression<Func<Guid, bool>> exp = Expression.Lambda<Func<Guid, bool>>(
                Expression.Block(
                    Expression.Call(threadSleepMethod, new[] {Expression.Constant(10)}),
                    Expression.Equal(parameter, Expression.Constant(guid), false, typeof(Guid).GetMethod("op_Equality"))
                    ),
                parameter);

            new Thread(Collect).Start();

            for(int iter = 0; iter < 100; ++iter)
                DoTestConstsAreNotFreedWhileRunning(exp, new Guid("2e224f5f-e392-4753-a19a-4304f226b965"));
        }

        [Test]
        public void TestConstsAreFreedAfterGarbageCollecting1()
        {
            var weakRef = DoTestConstsAreFreedAfterGarbageCollecting1();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.IsFalse(weakRef.IsAlive);
        }

        [Test]
        public void TestConstsAreFreedAfterGarbageCollecting2()
        {
            var weakRef = DoTestConstsAreFreedAfterGarbageCollecting2();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.IsFalse(weakRef.IsAlive);
        }

        [Test]
        public void TestMultiThread()
        {
            ParameterExpression list = Expression.Variable(typeof(List<string>));
            Expression listCreate = Expression.Assign(list, Expression.New(typeof(List<string>)));
            Expression<Func<TestClassA, TestClassB[]>> pathToArray = a => a.ArrayB;
            Expression<Func<TestClassB, string>> pathToS = b => b.S;
            Expression addToList = Expression.Call(list, typeof(List<string>).GetMethod("Add", new[] {typeof(string)}), pathToS.Body);
            MethodCallExpression forEach = Expression.Call(forEachMethod.MakeGenericMethod(new[] {typeof(TestClassB)}), pathToArray.Body, Expression.Lambda<Action<TestClassB>>(addToList, pathToS.Parameters));
            var body = Expression.Block(typeof(List<string>), new[] {list}, listCreate, forEach, list);
            var exp = Expression.Lambda<Func<TestClassA, List<string>>>(body, pathToArray.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            wasBug = false;
            var thread = new Thread(Run);
            thread.Start(f);
            Run(f);
            Assert.IsFalse(wasBug);
        }

        [Test, Ignore]
        public void TestPopInt32()
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(string), new[] {typeof(TestClassA)}, typeof(Test).Module, true);
            var il = method.GetILGenerator(); //new GroboIL(method);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Dup);
            il.EmitCall(OpCodes.Call, typeof(TestClassA).GetProperty("E", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), null);
            var temp = il.DeclareLocal(typeof(long));
            il.Emit(OpCodes.Stloc, temp);
            //il.Pop();
            il.EmitCall(OpCodes.Call, typeof(TestClassA).GetProperty("S", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), null);
            il.Emit(OpCodes.Ret);
            var func = (Func<TestClassA, string>)method.CreateDelegate(typeof(Func<TestClassA, string>));
            Assert.AreEqual("zzz", func(new TestClassA {S = "zzz"}));
        }

        [Test]
        public void TestDefault()
        {
            Expression<Func<long>> exp = Expression.Lambda<Func<long>>(Expression.Default(typeof(long)));
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f());
        }

        [Test]
        public void TestConvertToEnum()
        {
            Expression<Func<TestEnum, Enum>> exp = x => (Enum)x;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(TestEnum.Two, f(TestEnum.Two));
        }

        [Test]
        public void TestConvertFromEnum()
        {
            Expression<Func<Enum, TestEnum>> exp = x => (TestEnum)x;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(TestEnum.Two, f(TestEnum.Two));
        }

        [Test]
        public void TestConditional1()
        {
            Expression<Func<TestClassA, int>> exp = a => a.Bool ? 1 : -1;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(new TestClassA {Bool = true}));
            Assert.AreEqual(-1, f(null));
            Assert.AreEqual(-1, f(new TestClassA()));
        }

        [Test]
        public void TestConditional2()
        {
            Expression<Func<TestClassA, string>> path = a => a.B.S;
            Expression<Func<TestClassA, bool>> condition = a => a.S == "zzz";
            Expression assign = Expression.Assign(path.Body, Expression.Constant("qxx"));
            Expression test = new ParameterReplacer(condition.Parameters[0], path.Parameters[0]).Visit(condition.Body);
            Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(Expression.IfThenElse(test, assign, Expression.Default(typeof(void))), path.Parameters);
            var action = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {S = "zzz"};
            action(o);
            Assert.IsNotNull(o.B);
            Assert.AreEqual(o.B.S, "qxx");
        }

        [Test]
        public void TestConditional3()
        {
            Expression<Func<TestClassA, string>> path = a => a.B.S;
            Expression<Func<TestClassA, string>> path2 = a => a.S;
            Expression<Func<TestClassA, bool>> condition = a => a.S == "zzz";
            Expression assign1 = Expression.Assign(path.Body, Expression.Constant("qxx"));
            Expression assign2 = Expression.Assign(path.Body, Expression.Constant("qzz"));
            Expression test = new ParameterReplacer(condition.Parameters[0], path.Parameters[0]).Visit(condition.Body);
            Expression assign3 = Expression.Assign(new ParameterReplacer(path2.Parameters[0], path.Parameters[0]).Visit(path2.Body), Expression.Block(typeof(string), Expression.IfThenElse(test, assign1, assign2), path.Body));
            //Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(Expression.IfThenElse(test, assign1, assign2), path.Parameters);
            Expression<Action<TestClassA>> exp = Expression.Lambda<Action<TestClassA>>(assign3, path.Parameters);
            var action = LambdaCompiler.Compile(exp, CompilerOptions.All);
            var o = new TestClassA {S = "zzz"};
            action(o);
            Assert.IsNotNull(o.B);
            Assert.AreEqual(o.B.S, "qxx");
            Assert.AreEqual(o.S, "qxx");
        }

        [Test]
        public void TestConditional4()
        {
            Expression<Func<TestClassA, string>> exp = a => (a.S == "zzz" ? a.Y : a.Y2).ToString();
            var func = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("1", func(new TestClassA{Y = 1, Y2 = 2, S = "zzz"}));
            Assert.AreEqual("2", func(new TestClassA{Y = 1, Y2 = 2, S = "qxx"}));
        }

        [Test]
        public void TestToStringOfGuid()
        {
            Expression<Func<TestClassA, string>> exp = f => "_xxx_" + f.Guid.ToString();
            Func<TestClassA, string> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(new TestClassA {Guid = new Guid("8DCBF7DF-772A-4A9C-81F0-D4B25C183ACE")}), Is.EqualTo("_xxx_8DCBF7DF-772A-4A9C-81F0-D4B25C183ACE").IgnoreCase);
        }

        [Test]
        public void Test9()
        {
            Expression<Func<TestClassA, int>> exp = o => (o.B ?? new TestClassB {Y = 3}).Y;
            Func<TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(3));
            Assert.That(compiledExp(new TestClassA()), Is.EqualTo(3));
            Assert.That(compiledExp(new TestClassA {B = new TestClassB {Y = 2}}), Is.EqualTo(2));
        }

        [Test]
        public void Test23()
        {
            Expression<Func<TestClassA, bool>> exp = o => o.NullableBool == false;
            Func<TestClassA, bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(compiledExp(null));
            Assert.IsFalse(compiledExp(new TestClassA()));
        }

        [Test]
        public void TestNullableGuidNoCoalesce()
        {
            Expression<Func<TestClassA, bool>> exp = o => o.NullableGuid == null;
            Func<TestClassA, bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(compiledExp(new TestClassA {NullableGuid = Guid.Empty}));
        }

        [Test]
        public void TestCoalesce1()
        {
            Expression<Func<TestClassA, int>> exp = o => o.B.X ?? 1;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(null));
            Assert.AreEqual(1, f(new TestClassA()));
            Assert.AreEqual(1, f(new TestClassA {B = new TestClassB()}));
            Assert.AreEqual(2, f(new TestClassA {B = new TestClassB {X = 2}}));
        }

        [Test]
        public void TestCoalesce2()
        {
            Expression<Func<TestClassA, int?, int?>> exp = (o, x) => o.B.X ?? x;
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(null, f(null, null));
            Assert.AreEqual(1, f(null, 1));
            Assert.AreEqual(null, f(new TestClassA(), null));
            Assert.AreEqual(2, f(new TestClassA(), 2));
            Assert.AreEqual(null, f(new TestClassA {B = new TestClassB()}, null));
            Assert.AreEqual(3, f(new TestClassA {B = new TestClassB()}, 3));
            Assert.AreEqual(4, f(new TestClassA {B = new TestClassB {X = 4}}, null));
            Assert.AreEqual(4, f(new TestClassA {B = new TestClassB {X = 4}}, 5));
        }

        [Test]
        public void TestLazyEvaluation()
        {
            Expression<Func<TestClassA, bool>> exp = o => o.A != null && Y(o.A) > 0;
            Func<TestClassA, bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.False);
            Assert.That(compiledExp(new TestClassA()), Is.False);
            Assert.That(compiledExp(new TestClassA {A = new TestClassA()}), Is.False);
            Assert.That(compiledExp(new TestClassA {A = new TestClassA {Y = 1}}), Is.True);
        }

        [Test]
        public void TestLazyEvaluation2()
        {
            Expression<Func<int?, bool>> exp = o => o != null && o.ToString() == "1";
            Func<int?, bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.False);
            Assert.That(compiledExp(1), Is.True);
        }

        [Test]
        public void TestStaticMethod()
        {
            Expression<Func<TestClassA, TestClassA, int>> exp = (x, y) => NotExtension(x, y);
            Func<TestClassA, TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);

            Assert.That(compiledExp(new TestClassA(), new TestClassA()), Is.EqualTo(3), "!null,!null");
            Assert.That(compiledExp(new TestClassA(), null), Is.EqualTo(2), "!null,null");
            Assert.That(compiledExp(null, null), Is.EqualTo(1), "null,null");
        }

        [Test]
        public void TestStaticMethodAsSubChain()
        {
            Expression<Func<TestClassA, TestClassA, int>> exp = (x, y) => NotExtension2(x.A, y.A).Y;
            Func<TestClassA, TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);

            Assert.That(compiledExp(new TestClassA {A = new TestClassA()}, new TestClassA {A = new TestClassA()}), Is.EqualTo(3), "!null,!null");
            Assert.That(compiledExp(new TestClassA {A = new TestClassA()}, null), Is.EqualTo(2), "!null,null");
            Assert.That(compiledExp(null, null), Is.EqualTo(1), "null,null");
        }

        [Test]
        public void TestCompileExtendedExpressionWithClosure()
        {
            var closure = 11;
            Expression<Func<TestClassA, bool>> exp = o => closure == o.Y;
            Func<TestClassA, bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(new TestClassA {Y = 11}), Is.True);
        }

        [Test]
        public void TestNullableValidType()
        {
            Expression<Func<int?, string>> exp = o => o == null ? "" : o.ToString();
            Func<int?, string> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(compiledExp(null), "");
        }

        [Test]
        public void TestExtendFunctionArguments()
        {
            Expression<Func<TestClassA, int>> exp = o => zzz(o.X > 0);
            Func<TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, compiledExp(null));
            Assert.AreEqual(0, compiledExp(new TestClassA()));
            Assert.AreEqual(1, compiledExp(new TestClassA {X = 1}));
        }

        [Test]
        public void Test99()
        {
            Expression<Func<int[], bool>> exp = o => 11 == o[o.Count() - 1];
            Func<int[], bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(new[] {1, 11}), Is.True);
        }

        [Test]
        public void TestExtendForNullableValueTypes()
        {
            Expression<Func<int?, string>> exp = s => s.ToString();
            Func<int?, string> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(null, compiledExp(null));
            Assert.AreEqual("1", compiledExp(1));
        }

        [Test]
        public void TestExtendForNullableValueTypes2()
        {
            Expression<Func<int?, bool>> exp = s => s == 0;
            Func<int?, bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsFalse(compiledExp(null));
        }

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
            public TestEnum E { get; set; }
            public TestStructA[] StructAArray { get; set; }
            public string[] StringArray { get; set; }
            public int[] IntArray2;
            public int? X;
            public Guid Guid = Guid.Empty;
            public Guid? NullableGuid;
            public bool? NullableBool;
            public int Y;
            public int Y2;
            public bool Bool;
            public long Z;

            public TestStructA StructA;

            ~TestClassA()
            {
                S = null;
            }
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

            public int? X { get; set; }

            public readonly string S;
        }

        public class TestClassE
        {
            public string S { get; set; }
            public int X { get; set; }
        }

        public struct TestStructA
        {
            public TestStructB[] ArrayB { get; set; }
            public int? X { get; set; }
            public int Y { get; set; }
            public TestClassA A { get; set; }
            public string S;
        }

        public struct TestStructB
        {
            public string S { get; set; }
        }

        public enum TestEnum
        {
            Zero = 0,
            One = 1,
            Two = 2
        }

        private void DoTestConstsAreNotFreedWhileRunning(Expression<Func<Guid, bool>> lambda, Guid guid)
        {
            var func = LambdaCompiler.Compile(lambda, CompilerOptions.None);
            var actual = func(guid);
            Assert.IsTrue(actual);
        }

        private static int?[] CreateNullableIntArray(TestClassA a)
        {
            return new int?[a.Z];
        }

        private static void Qzz1(TestClassA a)
        {
            a.StructA.S = "zzz";
        }

        private static void Qzz2(TestClassA a, TestStructA aa)
        {
            a.StructAArray[1] = aa;
        }

        private static unsafe void Qzz3(TestClassA a, int x)
        {
            fixed(int* p = &a.IntArray[0])
                *(p + 1) = x;
        }

        private static string GetString()
        {
            int j = 0;
            for(int i = 0; i < 100000; ++i)
            {
                var o = new TestClassA {ArrayB = new TestClassB[100]};
                j += i * o.ArrayB.Length;
            }
            return j.ToString();
        }

        private void GenerateTrash()
        {
            for(;;)
            {
                var list = new List<TestClassA>();
                for(int i = 0; i < 1000; ++i)
                    list.Add(new TestClassA {ArrayB = new TestClassB[100], IntArray = new int[100]});
            }
        }

        private void Collect()
        {
            for(;;)
            {
                GC.Collect();
                Thread.Sleep(100);
            }
        }

        private static WeakReference DoTestConstsAreFreedAfterGarbageCollecting1()
        {
            var a = new TestClassA {S = "qxx"};
            var result = new WeakReference(a);
            Expression<Func<TestClassA, string>> path = o => o.S;
            var exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.Equal(path.Body, Expression.MakeMemberAccess(Expression.Constant(a), typeof(TestClassA).GetProperty("S"))), path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(new TestClassA {S = "qxx"}));
            Assert.IsFalse(f(new TestClassA {S = "qzz"}));
            return result;
        }

        private static WeakReference DoTestConstsAreFreedAfterGarbageCollecting2()
        {
            var a = new TestClassA {S = "qxx"};
            var result = new WeakReference(a);
            var aa = new TestStructA {A = a};
            Expression<Func<TestClassA, string>> path = o => o.S;
            var exp = Expression.Lambda<Func<TestClassA, bool>>(Expression.Equal(path.Body, Expression.MakeMemberAccess(Expression.MakeMemberAccess(Expression.Constant(aa), typeof(TestStructA).GetProperty("A")), typeof(TestClassA).GetProperty("S"))), path.Parameters);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(new TestClassA {S = "qxx"}));
            Assert.IsFalse(f(new TestClassA {S = "qzz"}));
            return result;
        }

        private void Run(object param)
        {
            var f = (Func<TestClassA, List<string>>)param;
            for(int i = 0; i < 1000000; ++i)
            {
                string s = Guid.NewGuid().ToString();
                var a = new TestClassA {ArrayB = new[] {new TestClassB {S = s}}};
                var list = f(a);
                if(list.Count == 0 || list[0] != s)
                    wasBug = true;
            }
        }

        public int zzz(bool qxx)
        {
            return qxx ? 1 : 0;
        }

        public static int NotExtension(TestClassA x, TestClassA y)
        {
            if(x == null) return 1;
            if(y == null) return 2;
            return 3;
        }

        public static TestClassA NotExtension2(TestClassA x, TestClassA y)
        {
            if(x == null) return new TestClassA {Y = 1};
            if(y == null) return new TestClassA {Y = 2};
            return new TestClassA {Y = 3};
        }

        public static int Y(TestClassA a)
        {
            return a.Y;
        }

        private static readonly MethodInfo threadSleepMethod = ((MethodCallExpression)((Expression<Action>)(() => Thread.Sleep(0))).Body).Method;

        private static readonly MethodInfo getStringMethod = ((MethodCallExpression)((Expression<Func<string>>)(() => GetString())).Body).Method;

        private volatile bool wasBug;

        private static readonly MethodInfo forEachMethod = ((MethodCallExpression)((Expression<Action<int[]>>)(ints => Array.ForEach(ints, null))).Body).Method.GetGenericMethodDefinition();
    }
}