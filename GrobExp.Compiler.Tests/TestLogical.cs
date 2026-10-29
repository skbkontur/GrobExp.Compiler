using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests
{
    [TestFixture]
    public class TestLogical
    {
        [Test]
        public void TestLogical1()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.NullableBool;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(compiledExp(null));
            ClassicAssert.IsNull(compiledExp(new TestClassA()));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {NullableBool = true}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {NullableBool = false}));
        }

        [Test]
        public void TestLogical2()
        {
            Expression<Func<TestClassA, bool?>> exp = o => !o.NullableBool;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(compiledExp(null));
            ClassicAssert.IsNull(compiledExp(new TestClassA()));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {NullableBool = true}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {NullableBool = false}));
        }

        [Test]
        public void TestLogical3()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.B.X > 0;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(compiledExp(null));
            ClassicAssert.IsNull(compiledExp(new TestClassA()));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {B = new TestClassB {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB {X = 1}}));
        }

        [Test]
        public void TestLogical4()
        {
            Expression<Func<TestClassA, bool?>> exp = o => !(o.B.X > 0);
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(compiledExp(null));
            ClassicAssert.IsNull(compiledExp(new TestClassA()));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB {X = -1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {B = new TestClassB {X = 1}}));
        }

        [Test]
        public void TestLogical5()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.B.X > 0 && o.A.X > 0;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(compiledExp(null));
            ClassicAssert.IsNull(compiledExp(new TestClassA()));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {B = new TestClassB {X = -1}}));
            ClassicAssert.IsNull(compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}, B = new TestClassB {X = 1}}));
        }

        [Test]
        public void TestLogical6()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.B.X > 0 || o.A.X > 0;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(compiledExp(null));
            ClassicAssert.IsNull(compiledExp(new TestClassA()));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB {X = 1}}));
            ClassicAssert.IsNull(compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = -1}, B = new TestClassB {X = -1}}));
        }

        [Test]
        public void TestLogical7()
        {
            Expression<Func<TestClassA, bool>> exp = o => o.B.X > 0 || o.A.X > 0;
            Func<TestClassA, bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsFalse(compiledExp(null));
            ClassicAssert.IsFalse(compiledExp(new TestClassA()));
            ClassicAssert.IsFalse(compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.IsFalse(compiledExp(new TestClassA {B = new TestClassB {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB {X = 1}}));
            ClassicAssert.IsFalse(compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = -1}, B = new TestClassB {X = -1}}));
        }

        [Test]
        public void TestLogical8()
        {
            Expression<Func<TestClassA, int>> exp = o => o.B.X > 0 || o.A.X > 0 ? 1 : 0;
            Func<TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, compiledExp(null));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA()));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA {B = new TestClassB {X = -1}}));
            ClassicAssert.AreEqual(1, compiledExp(new TestClassA {B = new TestClassB {X = 1}}));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(1, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA {A = new TestClassA {X = -1}, B = new TestClassB {X = -1}}));
        }

        [Test]
        public void TestLogical9()
        {
            Expression<Func<TestClassA, int>> exp = o => o.F(o.B.X > 0 || o.A.X > 0);
            Func<TestClassA, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(0, compiledExp(null));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA()));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA {B = new TestClassB {X = -1}}));
            ClassicAssert.AreEqual(1, compiledExp(new TestClassA {B = new TestClassB {X = 1}}));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(1, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(0, compiledExp(new TestClassA {A = new TestClassA {X = -1}, B = new TestClassB {X = -1}}));
        }

        [Test]
        public void TestLogical10()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.B.X > 0 || o.A.X > 0;
            ParameterExpression var = Expression.Variable(typeof(bool));
            var body = Expression.Block(typeof(bool), new[] {var}, Expression.Assign(var, Expression.Convert(exp.Body, typeof(bool))), var);
            Func<TestClassA, bool> compiledExp = LambdaCompiler.Compile(Expression.Lambda<Func<TestClassA, bool>>(body, exp.Parameters), CompilerOptions.All);
            ClassicAssert.IsFalse(compiledExp(null));
            ClassicAssert.IsFalse(compiledExp(new TestClassA()));
            ClassicAssert.IsFalse(compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.IsFalse(compiledExp(new TestClassA {B = new TestClassB {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB {X = 1}}));
            ClassicAssert.IsFalse(compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = -1}, B = new TestClassB {X = -1}}));
        }

        [Test]
        public void TestLogical11()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.B.Y > 0 && o.A.X > 0;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(false, compiledExp(null));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA()));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB {Y = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {B = new TestClassB {Y = -1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}, B = new TestClassB {Y = 1}}));
        }

        [Test]
        public void TestLogical12()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.B.Y > 0 || o.A.X > 0;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(compiledExp(null));
            ClassicAssert.IsNull(compiledExp(new TestClassA()));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB {Y = 1}}));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB {Y = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}, B = new TestClassB {Y = 1}}));
        }

        [Test]
        public void TestLogical13()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.A.X > 0 && o.B.Y > 0;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(false, compiledExp(null));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA()));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB {Y = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {B = new TestClassB {Y = -1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}, B = new TestClassB {Y = 1}}));
        }

        [Test]
        public void TestLogical14()
        {
            Expression<Func<TestClassA, bool?>> exp = o => o.A.X > 0 || o.B.Y > 0;
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.IsNull(compiledExp(null));
            ClassicAssert.IsNull(compiledExp(new TestClassA()));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB {Y = 1}}));
            ClassicAssert.IsNull(compiledExp(new TestClassA {B = new TestClassB {Y = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {A = new TestClassA {X = -1}}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {A = new TestClassA {X = 1}, B = new TestClassB {Y = 1}}));
        }

        [Test]
        public void TestLogical15()
        {
            Expression<Func<TestClassA, bool?>> exp = o => !(o.B.Y > 0);
            Func<TestClassA, bool?> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(true, compiledExp(null));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA()));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB()}));
            ClassicAssert.AreEqual(true, compiledExp(new TestClassA {B = new TestClassB {Y = -1}}));
            ClassicAssert.AreEqual(false, compiledExp(new TestClassA {B = new TestClassB {Y = 1}}));
        }

        [Test]
        public void TestLogical16()
        {
            Expression<Func<TestClassA, string>> exp = o => ((bool?)(o.X > 0)).ToString();
            Func<TestClassA, string> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            ClassicAssert.AreEqual(null, compiledExp(null));
            ClassicAssert.AreEqual(null, compiledExp(new TestClassA()));
            ClassicAssert.AreEqual("False", compiledExp(new TestClassA {X = -2}));
            ClassicAssert.AreEqual("True", compiledExp(new TestClassA {X = 1}));
        }

        public struct TestStructA
        {
            public string S { get; set; }
            public TestStructB[] ArrayB { get; set; }
            public int? X { get; set; }
            public int Y { get; set; }
        }

        public struct TestStructB
        {
            public string S { get; set; }
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
            public int? X;
            public Guid Guid = Guid.Empty;
            public Guid? NullableGuid;
            public bool? NullableBool;
            public int Y;
            public bool Bool;
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
    }
}