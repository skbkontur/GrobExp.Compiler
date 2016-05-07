using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestBase
    {
        protected TDelegate Compile<TDelegate>(Expression<TDelegate> lambda, CompilerOptions options) where TDelegate : class
        {
            return LambdaCompiler.Compile(lambda, options);
        }

        protected TDelegate CompileToMethod<TDelegate>(Expression<TDelegate> lambda, CompilerOptions options) where TDelegate : class
        {
            var typeBuilder = TestPerformance.Module.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Public);
            var method = typeBuilder.DefineMethod("lambda", MethodAttributes.Public | MethodAttributes.Static, lambda.ReturnType, lambda.Parameters.Select(parameter => parameter.Type).ToArray());
            LambdaCompiler.CompileToMethod(lambda, method, options);
            var type = typeBuilder.CreateType();
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(TDelegate), null, TestPerformance.Module, true);
            using(var il = new GroboIL(dynamicMethod))
            {
                il.Ldnull();
                il.Ldftn(type.GetMethod("lambda"));
                il.Newobj(typeof(TDelegate).GetConstructor(new[] {typeof(object), typeof(IntPtr)}));
                il.Ret();
            }
            return ((Func<TDelegate>)dynamicMethod.CreateDelegate(typeof(Func<TDelegate>)))();
        }
    }
}