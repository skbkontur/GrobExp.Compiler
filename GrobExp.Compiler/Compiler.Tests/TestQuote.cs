using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestQuote
    {
        [Test]
        public void Test1()
        {
            Expression<Func<int, int>> exp = i => F(j => j * j);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(25, f(2));
        }

        [Test]
        public void Test2()
        {
            Expression<Func<int, int>> exp = i => F(j => j * i);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(10, f(2));
        }

        [Test]
        public void Test3()
        {
            ParameterExpression x = Expression.Parameter(typeof(int));
            ParameterExpression y = Expression.Parameter(typeof(int));
            Expression body = Expression.Call(typeof(TestQuote).GetMethod("F2", BindingFlags.Public | BindingFlags.Static), Expression.Quote(Expression.Lambda<Func<int, IRuntimeVariables>>(Expression.RuntimeVariables(x, y), y)));
            Expression<Func<int, int>> exp = Expression.Lambda<Func<int, int>>(body, x);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(10, f(10));
        }

        public static int F(Expression<Func<int, int>> exp)
        {
            return LambdaCompiler.Compile(exp, CompilerOptions.All)(5);
        }

        public static int F2(Expression<Func<int, IRuntimeVariables>> exp)
        {
            return (int)LambdaCompiler.Compile(exp, CompilerOptions.All)(5)[0];
        }
    }
}