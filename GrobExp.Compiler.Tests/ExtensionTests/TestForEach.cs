using System;
using System.Linq.Expressions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace GrobExp.Compiler.Tests.ExtensionTests
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
            ClassicAssert.AreEqual(6, f(new[] {1, -1, 2, -2, 3, -3}));
            ClassicAssert.AreEqual(6, f(new[] {1, 2, 3}));
            ClassicAssert.AreEqual(0, f(new[] {-1, -2, -3}));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            ClassicAssert.AreEqual(6, f(new[] {1, -1, 2, -2, 3, -3}));
            ClassicAssert.AreEqual(6, f(new[] {1, 2, 3}));
            ClassicAssert.AreEqual(0, f(new[] {-1, -2, -3}));

            f = exp.Compile();
            ClassicAssert.AreEqual(6, f(new[] {1, -1, 2, -2, 3, -3}));
            ClassicAssert.AreEqual(6, f(new[] {1, 2, 3}));
            ClassicAssert.AreEqual(0, f(new[] {-1, -2, -3}));
        }

        public static ForEachExpression ForEach(Expression enumerable, Type elementType, LambdaExpression body)
        {
            return new ForEachExpression(enumerable, elementType, body);
        }
    }
}