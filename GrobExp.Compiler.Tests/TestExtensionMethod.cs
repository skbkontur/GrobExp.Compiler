using System;
using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

namespace GrobExp.Compiler.Tests
{
    [TestFixture]
    public class TestExtensionMethod
    {
        [Test]
        public void TestEnum()
        {
            Expression<Func<Zerg, bool>> exp = zerg => zerg.Flies();
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(Zerg.Mutalisk));
            Assert.IsFalse(f(Zerg.Zergling));
        }

        [Test]
        public void TestNullable()
        {
            Expression<Func<Zerg?, bool>> exp = zerg => zerg.AttacksAir();
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(Zerg.Mutalisk));
            Assert.IsFalse(f(Zerg.Zergling));
            Assert.IsFalse(f(null));
        }

        [Test]
        public void TestLinq1()
        {
            Expression<Func<int[], bool>> exp = ints => ints.All(i => i > 0);
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(null));
            Assert.IsTrue(f(new int[0]));
            Assert.IsTrue(f(new[] {1}));
            Assert.IsFalse(f(new[] {-1}));
        }

        [Test]
        public void TestLinq2()
        {
            Expression<Func<int[], int>> exp = ints => ints.Select(i => i * i).FirstOrDefault();
            var f = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.AreEqual(0, f(null));
            Assert.AreEqual(0, f(new int[0]));
            Assert.AreEqual(9, f(new[] {3}));
        }

        /// <summary>
        ///     Test validates that null reference check not applied to the first argument of an extension method
        /// </summary>
        [Test]
        public void TestNullTargetArgument()
        {
            Expression<Func<Zurg, bool>> exp = zurg => zurg.IsNull();
            Func<Zurg, bool> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(null), Is.EqualTo(true));
            Assert.That(compiledExp(new Zurg()), Is.EqualTo(false));
        }

        [Test]
        public void TestRefParameters()
        {
            Expression<Func<Zurg, int, int, int>> exp = (o, i, j) => o.Nonsense(ref i, j) ? -1 : j;
            Func<Zurg, int, int, int> compiledExp = LambdaCompiler.Compile(exp, CompilerOptions.All);
            Assert.That(compiledExp(new Zurg {Length = 1}, 1, 2), Is.EqualTo(2));
        }
    }

    public enum Zerg
    {
        Drone = 1,
        Overlord = 2,
        Zergling = 3,
        Hydralisk = 4,
        Mutalisk = 5,
        Ultralisk = 6,
        Guardian = 7,
        Devourer = 8,
        Lurker = 9
    }

    public class Zurg
    {
        public int Length { get; set; }
    }

    public static class ZergExtensions
    {
        public static bool Flies(this Zerg zerg)
        {
            return zerg == Zerg.Overlord || zerg == Zerg.Mutalisk || zerg == Zerg.Guardian || zerg == Zerg.Devourer;
        }

        public static bool AttacksAir(this Zerg? zerg)
        {
            return zerg == Zerg.Hydralisk || zerg == Zerg.Mutalisk || zerg == Zerg.Devourer;
        }
    }

    public static class ZurgExtensions
    {
        public static bool IsNull(this Zurg zurg)
        {
            return zurg == null;
        }

        public static bool Nonsense(this Zurg zurg, ref int a, int b)
        {
            a = zurg.Length;
            return a == b;
        }
    }
}