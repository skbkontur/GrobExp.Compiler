using System;
using System.Linq.Expressions;

using GrobExp.Compiler;

using Microsoft.CSharp.RuntimeBinder;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestDynamic
    {
        [Test]
        public void Test()
        {
            var x = Expression.Parameter(typeof(object), "x");
            var y = Expression.Parameter(typeof(object), "y");
            var binder = Binder.BinaryOperation(
                CSharpBinderFlags.None, ExpressionType.Add, typeof(TestDynamic),
                new[]
                    {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                    });

            var exp = Expression.Lambda<Func<object, object, object>>(
                Expression.Dynamic(binder, typeof(object), x, y),
                new[] {x, y}
                );

            Func<dynamic, dynamic, dynamic> f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(3, f(1, 2));

            f = LambdaCompiler.Compile(exp, CompilerOptions.None);
            Assert.AreEqual(3, f(1, 2));

            f = exp.Compile();
            Assert.AreEqual(3, f(1, 2));
        }
    }
}