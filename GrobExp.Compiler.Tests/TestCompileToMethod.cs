using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using NUnit.Framework;

namespace GrobExp.Compiler.Tests
{
    [TestFixture]
    public class TestCompileToMethod
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            var assemblyName = "TestAssembly";
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
        }

        [Test]
        public void TestNullablePrimitiveConstant()
        {
            var typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class);
            var methodName = nameof(TestNullablePrimitiveConstant);
            var methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static);

            // Cannot declare constant of nullable type via Expression<Func<>> interface
            // Expression<Func<int?, string>> expression = x => x == (int? 2) ? "two" : x.ToString();
            var parameter = Expression.Parameter(typeof(int?));
            var constant = Expression.Constant((int?)2, typeof(int?));
            var condition = Expression.Condition(Expression.Equal(parameter, constant),
                                                 Expression.Constant("two", typeof(string)),
                                                 Expression.Call(parameter, typeof(int?).GetMethod("ToString", Type.EmptyTypes)));
            var lambda = Expression.Lambda(condition, parameter);

            LambdaCompiler.CompileToMethod(lambda, methodBuilder, CompilerOptions.All);

            var type = typeBuilder.CreateType();
            var method = type.GetMethod(methodName, new[] {typeof(int?)});
            Assert.That(method.Invoke(null, new object[] {new int?(2)}), Is.EqualTo("two"));
            Assert.That(method.Invoke(null, new object[] {new int?(3)}), Is.EqualTo("3"));
        }

        [Test]
        public void TestEnumConstant()
        {
            var typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class);
            var methodName = nameof(TestEnumConstant);
            var methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static);

            // Cannot use Expression<Func<>> interface here because enum type gets optimized to int
            // Expression<Func<TestEnum, string>> expression = x => x == TestEnum.Two ? "2" : x.ToString();
            var parameter = Expression.Parameter(typeof(TestEnum));
            var constant = Expression.Constant(TestEnum.Two, typeof(TestEnum));
            var condition = Expression.Condition(Expression.Equal(parameter, constant),
                                                 Expression.Constant("2", typeof(string)),
                                                 Expression.Call(parameter, typeof(object).GetMethod("ToString", Type.EmptyTypes)));
            var lambda = Expression.Lambda(condition, parameter);

            LambdaCompiler.CompileToMethod(lambda, methodBuilder, CompilerOptions.All);

            var type = typeBuilder.CreateType();
            var method = type.GetMethod(methodName, new[] {typeof(TestEnum)});
            Assert.That(method.Invoke(null, new object[] {TestEnum.Two}), Is.EqualTo("2"));
            Assert.That(method.Invoke(null, new object[] {TestEnum.One}), Is.EqualTo("One"));
        }

        [Test]
        public void TestDecimalConstant()
        {
            var typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class);
            var methodName = nameof(TestDecimalConstant);
            var methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static);

            Expression<Func<decimal, string>> expression = x => x == 2m ? "two" : x.ToString();
            LambdaCompiler.CompileToMethod(expression, methodBuilder, CompilerOptions.All);

            var type = typeBuilder.CreateType();
            var method = type.GetMethod(methodName, new[] {typeof(decimal)});
            Assert.That(method.Invoke(null, new object[] {2m}), Is.EqualTo("two"));
            Assert.That(method.Invoke(null, new object[] {3m}), Is.EqualTo("3"));
        }

        public enum TestEnum
        {
            One,
            Two,
        }

        private ModuleBuilder moduleBuilder;
    }
}