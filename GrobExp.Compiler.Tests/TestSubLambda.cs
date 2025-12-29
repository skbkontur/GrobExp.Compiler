using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests
{
    public class TestSubLambda : TestBase
    {
        [Test]
        public void TestSubLambda1()
        {
            Expression<Func<TestClassA, bool>> exp = a => a.ArrayB.Any(b => b.S == a.S);
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsTrue(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB {S = "zzz"},}}));
            ClassicAssert.IsFalse(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB(),}}));
        }

        [Test]
        public void TestSubLambda1x()
        {
            Expression<Func<TestClassA, bool>> exp = a => a.ArrayB.Any(b => b.S == "zzz");
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsTrue(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB {S = "zzz"},}}));
            ClassicAssert.IsFalse(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB(),}}));
        }

        [Test]
        public void Test()
        {
            if (asdfsadf())
            {
                Console.WriteLine("zzz");
            }
            else
            {
                Console.WriteLine("qxx");
            }
        }

        private static bool asdfsadf()
        {
            return 1 == 1;
        }

        [Test]
        public void TestSubLambda2()
        {
            Expression<Func<TestClassA, IEnumerable<TestClassB>>> exp = a => a.ArrayB.Where(b => b.S == a.S);
            Expression where = exp.Body;
            ParameterExpression temp = Expression.Variable(typeof(IEnumerable<TestClassB>));
            Expression assignTemp = Expression.Assign(temp, where);
            Expression assignS = Expression.Assign(Expression.MakeMemberAccess(exp.Parameters[0], typeof(TestClassA).GetProperty("S", BindingFlags.Public | BindingFlags.Instance)), Expression.Constant("zzz"));
            Expression any = Expression.Call(anyMethod.MakeGenericMethod(typeof(TestClassB)), temp);
            var exp2 = Expression.Lambda<Func<TestClassA, bool>>(Expression.Block(typeof(bool), new[] {temp}, assignTemp, assignS, any), exp.Parameters);

            var f = Compile(exp2, CompilerOptions.All);
            ClassicAssert.IsTrue(f(new TestClassA {S = "qzz", ArrayB = new[] {new TestClassB {S = "zzz"},}}));
        }

        [Test]
        public void TestSubLambda2x()
        {
            Expression<Func<TestStructA, IEnumerable<TestStructB>>> exp = a => a.ArrayB.Where(b => b.S == a.S);
            Expression where = exp.Body;
            ParameterExpression temp = Expression.Variable(typeof(IEnumerable<TestStructB>));
            Expression assignTemp = Expression.Assign(temp, where);
            Expression assignS = Expression.Assign(Expression.MakeMemberAccess(exp.Parameters[0], typeof(TestStructA).GetProperty("S", BindingFlags.Public | BindingFlags.Instance)), Expression.Constant("zzz"));
            Expression any = Expression.Call(anyMethod.MakeGenericMethod(typeof(TestStructB)), temp);
            var exp2 = Expression.Lambda<Func<TestStructA, bool>>(Expression.Block(typeof(bool), new[] {temp}, assignTemp, assignS, any), exp.Parameters);

            var f = Compile(exp2, CompilerOptions.All);
            ClassicAssert.IsTrue(f(new TestStructA {S = "qzz", ArrayB = new[] {new TestStructB {S = "zzz"},}}));
        }

        [Test]
        public void TestSubLambda2y()
        {
            ParameterExpression parameterB = Expression.Parameter(typeof(TestStructB));
            TestStructA aaa = default(TestStructA);
            Expression<Func<TestStructB, bool>> predicate = Expression.Lambda<Func<TestStructB, bool>>(Expression.Equal(Expression.MakeMemberAccess(parameterB, typeof(TestStructB).GetProperty("Y")), Expression.MakeMemberAccess(Expression.Constant(aaa), typeof(TestStructA).GetProperty("Y"))), parameterB);

            ParameterExpression parameterA = Expression.Parameter(typeof(TestStructA));
            Expression any = Expression.Call(anyWithPredicateMethod.MakeGenericMethod(typeof(TestStructB)), Expression.MakeMemberAccess(parameterA, typeof(TestStructA).GetProperty("ArrayB")), predicate);
            Expression<Func<TestStructA, bool>> exp = Expression.Lambda<Func<TestStructA, bool>>(any, parameterA);
            var f = Compile(exp, CompilerOptions.All);
            aaa.Y = 1;
            ClassicAssert.IsFalse(f(new TestStructA {ArrayB = new[] {new TestStructB {Y = 1},}}));
        }

        [Test]
        public void TestSubLambda3()
        {
            Expression<Func<TestClassA, int>> exp = data => data.ArrayB.SelectMany(b => b.C.ArrayD, (classB, classD) => classD.ArrayE.FirstOrDefault(c => c.S == "zzz").X).Where(i => i > 0).FirstOrDefault();
            var f = Compile(exp, CompilerOptions.All);
            var a = new TestClassA
                {
                    ArrayB = new[]
                        {
                            new TestClassB
                                {
                                    C = new TestClassC
                                        {
                                            ArrayD = new[]
                                                {
                                                    new TestClassD
                                                        {
                                                            ArrayE = new[]
                                                                {
                                                                    new TestClassE {S = "qxx", X = -1},
                                                                    new TestClassE {S = "zzz", X = -1},
                                                                }
                                                        },
                                                }
                                        }
                                },
                            new TestClassB
                                {
                                    C = new TestClassC
                                        {
                                            ArrayD = new[]
                                                {
                                                    new TestClassD
                                                        {
                                                            ArrayE = new[]
                                                                {
                                                                    new TestClassE {S = "qxx", X = -1},
                                                                    new TestClassE {S = "zzz", X = 1},
                                                                }
                                                        },
                                                }
                                        }
                                },
                        }
                };
            ClassicAssert.AreEqual(1, f(a));
            ClassicAssert.AreEqual(0, f(null));
        }

        [Test]
        public void TestSubLambda4()
        {
            Expression<Func<TestClassA, IEnumerable<TestClassB>>> exp = a => a.ArrayB.Where(b => b.S == a.B.C.S);
            Expression where = exp.Body;
            ParameterExpression temp = Expression.Variable(typeof(IEnumerable<TestClassB>));
            Expression assignTemp = Expression.Assign(temp, where);
            Expression<Func<TestClassA, string>> path = a => a.B.C.S;
            Expression left = new ParameterReplacer(path.Parameters[0], exp.Parameters[0]).Visit(path.Body);
            Expression assignS = Expression.Assign(left, Expression.Constant("zzz"));
            Expression any = Expression.Call(anyMethod.MakeGenericMethod(typeof(TestClassB)), temp);
            var exp2 = Expression.Lambda<Func<TestClassA, bool>>(Expression.Block(typeof(bool), new[] {temp}, assignTemp, assignS, any), exp.Parameters);

            var f = Compile(exp2, CompilerOptions.All);
            ClassicAssert.IsTrue(f(new TestClassA {ArrayB = new[] {new TestClassB {S = "zzz"},}}));
        }

        [Test]
        public void TestSubLambda5()
        {
            Expression<Func<TestClassA, bool>> exp = a => a.ArrayB.Any(b => b.S == a.S && b.C.ArrayD.All(d => d.S == b.S && d.ArrayE.Any(e => e.S == a.S && e.S == b.S && e.S == d.S)));
            var f = Compile(exp, CompilerOptions.All);
            ClassicAssert.IsTrue(f(new TestClassA
                {
                    S = "zzz",
                    ArrayB = new[]
                        {
                            new TestClassB
                                {
                                    S = "zzz",
                                    C = new TestClassC
                                        {
                                            ArrayD = new[]
                                                {
                                                    new TestClassD {S = "zzz", ArrayE = new[] {new TestClassE {S = "zzz"},}},
                                                    new TestClassD {S = "zzz", ArrayE = new[] {new TestClassE {S = "zzz"},}}
                                                }
                                        }
                                },
                        }
                }));
            ClassicAssert.IsFalse(f(new TestClassA
                {
                    S = "zzz",
                    ArrayB = new[]
                        {
                            new TestClassB
                                {
                                    S = "zzz",
                                    C = new TestClassC
                                        {
                                            ArrayD = new[]
                                                {
                                                    new TestClassD {S = "qxx", ArrayE = new[] {new TestClassE {S = "zzz"},}},
                                                    new TestClassD {S = "zzz", ArrayE = new[] {new TestClassE {S = "zzz"},}}
                                                }
                                        }
                                },
                        }
                }));
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
            public int Y { get; set; }
        }

        private static readonly MethodInfo anyMethod = ((MethodCallExpression)((Expression<Func<IEnumerable<int>, bool>>)(ints => ints.Any())).Body).Method.GetGenericMethodDefinition();

        private static readonly MethodInfo anyWithPredicateMethod = ((MethodCallExpression)((Expression<Func<IEnumerable<int>, bool>>)(ints => ints.Any(i => i == 0))).Body).Method.GetGenericMethodDefinition();
        private static Func<int, int, int> func = (x, y) => x + y;

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
            public int Z;
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

            public string S;
        }

        public class TestClassE
        {
            public string S { get; set; }
            public int X { get; set; }
        }
    }
}