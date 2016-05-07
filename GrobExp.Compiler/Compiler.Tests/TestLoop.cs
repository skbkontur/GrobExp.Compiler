using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestLoop
    {
        [Test]
        public void Test1()
        {
            ParameterExpression value = Expression.Parameter(typeof(int), "value");
            ParameterExpression result = Expression.Parameter(typeof(int), "result");
            LabelTarget label = Expression.Label(typeof(int));
            BlockExpression block = Expression.Block(
                new[] {result},
                Expression.Assign(result, Expression.Constant(1)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.GreaterThan(value, Expression.Constant(1)),
                        Expression.MultiplyAssign(result,
                                                  Expression.PostDecrementAssign(value)),
                        Expression.Break(label, result)
                        ),
                    label
                    )
                );
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(block, value);

            Func<int, int> f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(120, f(5));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(120, f(5));

            f = exp.Compile();
            Assert.AreEqual(1, f(0));
            Assert.AreEqual(120, f(5));
        }

        [Test]
        public void Test1x()
        {
            ParameterExpression value = Expression.Parameter(typeof(int), "value");
            ParameterExpression result = Expression.Parameter(typeof(int), "result");
            LabelTarget label = Expression.Label(typeof(int));
            BlockExpression block = Expression.Block(
                new[] {result},
                Expression.Assign(result, Expression.Constant(1)),
                Expression.Loop(
                    Expression.Block(typeof(void), new[] {
                    Expression.IfThenElse(
                        Expression.GreaterThan(value, Expression.Constant(1)),
                        Expression.MultiplyAssign(result,
                                                  Expression.PostDecrementAssign(value)),
                        Expression.Break(label, result)
                        )
                    }),
                    label
                    )
                );
            Expression<Func<int, string>> exp = Expression.Lambda<Func<int, string>>(Expression.Call(block, "ToString", Type.EmptyTypes), value);

            Func<int, string> f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual("1", f(0));
            Assert.AreEqual("120", f(5));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            Assert.AreEqual("1", f(0));
            Assert.AreEqual("120", f(5));

            f = exp.Compile();
            Assert.AreEqual("1", f(0));
            Assert.AreEqual("120", f(5));
        }

        [Test]
        public void Test2()
        {
            ParameterExpression array = Expression.Parameter(typeof(int[]), "array");
            ParameterExpression index = Expression.Parameter(typeof(int), "index");
            ParameterExpression result = Expression.Parameter(typeof(int), "result");
            ParameterExpression item = Expression.Parameter(typeof(int), "item");
            LabelTarget breakLabel = Expression.Label(typeof(int));
            LabelTarget continueLabel = Expression.Label();
            BlockExpression block = Expression.Block(
                new[] {result, index},
                Expression.Assign(result, Expression.Constant(0)),
                Expression.Assign(index, Expression.Constant(-1)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(Expression.PreIncrementAssign(index), Expression.ArrayLength(array)),
                        Expression.Block(
                            new[] {item},
                            Expression.Assign(item, Expression.ArrayIndex(array, index)),
                            Expression.IfThen(
                                Expression.LessThanOrEqual(item, Expression.Constant(0)),
                                Expression.Continue(continueLabel)),
                            Expression.AddAssign(result, item)
                            ),
                        Expression.Break(breakLabel, result)
                        ),
                    breakLabel, continueLabel
                    )
                );
            Expression<Func<int[], int>> exp = Expression.Lambda<Func<int[], int>>(block, array);

            Func<int[], int> f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(6, f(new[] {1, -1, 2, -2, 3, -3}));
            Assert.AreEqual(6, f(new[] {1, 2, 3}));
            Assert.AreEqual(0, f(new[] {-1, -2, -3}));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            Assert.AreEqual(6, f(new[] {1, -1, 2, -2, 3, -3}));
            Assert.AreEqual(6, f(new[] {1, 2, 3}));
            Assert.AreEqual(0, f(new[] {-1, -2, -3}));

            f = exp.Compile();
            Assert.AreEqual(6, f(new[] {1, -1, 2, -2, 3, -3}));
            Assert.AreEqual(6, f(new[] {1, 2, 3}));
            Assert.AreEqual(0, f(new[] {-1, -2, -3}));
        }
    }
}