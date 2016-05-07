using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests.ExtensionTests
{
    [TestFixture]
    public class TestForEach
    {
        [Test]
        public void Test1()
        {
            ParameterExpression array = Expression.Parameter(typeof(int[]), "array");
            ParameterExpression result = Expression.Parameter(typeof(int), "result");
            ParameterExpression item = Expression.Parameter(typeof(int), "item");
            BlockExpression block = Expression.Block(
                new[] {result},
                Expression.Assign(result, Expression.Constant(0)),
                ForEach(
                    array, typeof(int),
                    Expression.Lambda(
                        Expression.IfThen(
                            Expression.GreaterThan(item, Expression.Constant(0)),
                            Expression.AddAssign(result, item)),
                        item)
                    ),
                result);
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

        public static ForEachExpression ForEach(Expression enumerable, Type elementType, LambdaExpression body)
        {
            return new ForEachExpression(enumerable, elementType, body);
        }
    }
}