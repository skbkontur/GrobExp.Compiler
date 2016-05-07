using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;

using GrEmit;

using GrobExp.Compiler;

using Microsoft.CSharp.RuntimeBinder;

using NUnit.Framework;

using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace Compiler.Tests
{
    public class TestSubLambda : TestBase
    {
        [Test]
        public void TestSubLambda1()
        {
            Expression<Func<TestClassA, bool>> exp = a => a.ArrayB.Any(b => b.S == a.S);
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB {S = "zzz"},}}));
            Assert.IsFalse(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB(),}}));
        }

        [Test]
        public void TestSubLambda1x()
        {
            Expression<Func<TestClassA, bool>> exp = a => a.ArrayB.Any(b => b.S == "zzz");
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB {S = "zzz"},}}));
            Assert.IsFalse(f(new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB(),}}));
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
            Assert.IsTrue(f(new TestClassA {S = "qzz", ArrayB = new[] {new TestClassB {S = "zzz"},}}));
        }

        [Test, Ignore]
        public void CompileAndSave()
        {
            /*Expression<Func<TestStructA, IEnumerable<TestStructB>>> exp = a => a.ArrayB.Where(b => b.S == a.S);
            Expression where = exp.Body;
            ParameterExpression temp = Expression.Variable(typeof(IEnumerable<TestStructB>));
            Expression assignTemp = Expression.Assign(temp, where);
            Expression assignS = Expression.Assign(Expression.MakeMemberAccess(exp.Parameters[0], typeof(TestStructA).GetProperty("S", BindingFlags.Public | BindingFlags.Instance)), Expression.Constant("zzz"));
            Expression any = Expression.Call(anyMethod.MakeGenericMethod(typeof(TestStructB)), temp);
            var exp2 = Expression.Lambda<Func<TestStructA, bool>>(Expression.Block(typeof(bool), new[] { temp }, assignTemp, assignS, any), exp.Parameters);*/
            //Expression<Func<TestClassA, int?>> exp = o => o.ArrayB[0].C.ArrayD[0].X;
//            ParameterExpression a = Expression.Parameter(typeof(double?));
//            ParameterExpression b = Expression.Parameter(typeof(double?));
//            var exp = Expression.Lambda<Func<double?, double?, double?>>(Expression.Power(a, b), a, b);
//            ParameterExpression parameter = Expression.Parameter(typeof(int?));
//            var exp = Expression.Lambda<Func<int?, int?>>(Expression.Increment(parameter), parameter);
//            var parameter = Expression.Parameter(typeof(object));
//            var exp = Expression.Lambda<Func<object, int?>>(Expression.Unbox(parameter, typeof(int?)), parameter);
//            Expression<Func<TestClassA, int>> exp = o => func(o.Y, o.Z);
//            Expression<Func<int, int, int>> lambda = (x, y) => x + y;
//            ParameterExpression parameter = Expression.Parameter(typeof(TestClassA));
//            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(Expression.Invoke(lambda, Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("Y")), Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("Z"))), parameter);

//            var x = Expression.Parameter(typeof(object), "x");
//            var y = Expression.Parameter(typeof(object), "y");
//            var binder = Binder.BinaryOperation(
//                CSharpBinderFlags.None, ExpressionType.Add, typeof(TestDynamic),
//                new[]
//                    {
//                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
//                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
//                    });
//            var exp = Expression.Lambda<Func<object, object, object>>(
//                Expression.Dynamic(binder, typeof(object), x, y),
//                new[] {x, y}
//                );

            //Expression<Func<TestClassA, bool>> exp = a => a.ArrayB.Any(b => b.S == a.S);
            Expression<Func<TestClassA, bool?>> exp = o => o.A.X > 0 && o.B.Y > 0;

//            int? guid = 5;
//            ParameterExpression parameter = Expression.Parameter(typeof(int?));
//            Expression<Func<int?, bool>> exp = Expression.Lambda<Func<int?, bool>>(
//                Expression.Block(
//                    Expression.Call(threadSleepMethod, new[] { Expression.Constant(10) }),
//                    Expression.Equal(parameter, Expression.Constant(guid, typeof(int?)))
//                    ),
//                parameter);

            CompileAndSave(exp);
        }

        private static readonly MethodInfo threadSleepMethod = ((MethodCallExpression)((Expression<Action>)(() => Thread.Sleep(0))).Body).Method;

        [Test, Ignore]
        public void TestDebugInfo()
        {
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("foo"), AssemblyBuilderAccess.RunAndSave);

            var mod = asm.DefineDynamicModule("mymod", "tmp.dll", true);
            var type = mod.DefineType("baz", TypeAttributes.Public | TypeAttributes.Class);
            var meth = type.DefineMethod("go", MethodAttributes.Public | MethodAttributes.Static, typeof(int), Type.EmptyTypes);

            var filename = "TestDebug2.txt";

            var sdi = Expression.SymbolDocument(filename, Guid.Empty, Guid.Empty, Guid.Empty);

            var di = Expression.DebugInfo(sdi, 2, 2, 2, 13);

            var exp = Expression.Divide(Expression.Constant(2), Expression.Subtract(Expression.Constant(4), Expression.Constant(4)));
            var block = Expression.Block(di, exp);

            var gen = DebugInfoGenerator.CreatePdbGenerator();

            LambdaExpression lambda = Expression.Lambda(block, new ParameterExpression[0]);
            LambdaCompiler.CompileToMethod(lambda, meth, gen, CompilerOptions.All);
            //lambda.CompileToMethod(meth, gen);

            var newtype = type.CreateType();
            asm.Save("tmp.dll");
            newtype.GetMethod("go").Invoke(null, new object[0]);
            //meth.Invoke(null, new object[0]);
            //lambda.DynamicInvoke(new object[0]);
            Console.WriteLine(" ");
        }

        [Test, Ignore]
        public void TestDebugInfo123()
        {
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("foo"), AssemblyBuilderAccess.RunAndSave);

            var mod = asm.DefineDynamicModule("mymod", "tmp2.dll", true);
            var type = mod.DefineType("baz", TypeAttributes.Public | TypeAttributes.Class);
            var meth = type.DefineMethod("go", MethodAttributes.Public | MethodAttributes.Static, typeof(int), Type.EmptyTypes);

            var filename = "test_log.txt";

            var sdi = Expression.SymbolDocument(filename, Guid.Empty, Guid.Empty, Guid.Empty);

            //var di = Expression.DebugInfo(sdi, 2, 2, 2, 13);

            var variable = Expression.Parameter(typeof(int), "x");
            var boolVar = Expression.Variable(typeof(bool));
            var two = Expression.Constant(2);
            var zero = Expression.Constant(0);
            var temp = Expression.Variable(typeof(int));

            var tst = Expression.Block(Expression.DebugInfo(sdi, 4, 13, 4, 19), Expression.Equal(variable, zero)/*, new TypedDebugInfoExpression(sdi, Expression.Equal(variable, zero), 5, 9, 5, 10)*/);
            var iftrue = Expression.Block(Expression.DebugInfo(sdi, 7, 13, 7, 19), Expression.Add(variable, two)/*new TypedDebugInfoExpression(sdi, Expression.Add(variable, two), 8, 9, 8, 10)*/);
            var iffalse = Expression.Block(Expression.DebugInfo(sdi, 10, 13, 10, 19), Expression.Multiply(variable, two)/*, new TypedDebugInfoExpression(sdi, Expression.Multiply(variable, two), 11, 9, 11, 10)*/);
            //var iftrue = Expression.Block(Expression.DebugInfo(sdi, 10, 20, 10, 26), variable, Expression.DebugInfo(sdi, 10, 20, 10, 26));
            //var iffalse = Expression.Block(Expression.DebugInfo(sdi, 14, 20, 14, 26), variable, Expression.DebugInfo(sdi, 14, 20, 14, 26));
            var exp = Expression.Condition(tst, iftrue, iffalse);

            /*
            var returnTarget = Expression.Label(typeof(int));
            var returnExpression = Expression.Return(returnTarget, exp, typeof(int));
            var returnLabel = Expression.Label(returnTarget, Expression.Constant(0));
            */

            var block = Expression.Block(Expression.DebugInfo(sdi, 3, 9, 3, 15), new TypedDebugInfoExpression(sdi, exp, 12, 5, 12, 6));
            //var block = Expression.Block(Expression.DebugInfo(sdi, 4, 16, 17, 10), Expression.Assign(temp, exp)));
            var kek = Expression.Lambda(block, variable);

            var gen = DebugInfoGenerator.CreatePdbGenerator();

            //LambdaExpression lambda = Expression.Lambda(block);
            LambdaCompiler.CompileToMethod(kek, meth, gen, CompilerOptions.All);
            //lambda.CompileToMethod(meth, gen);

            //meth.DefineParameter(1, ParameterAttributes.In, "$x");

            var newtype = type.CreateType();
            asm.Save("tmp2.dll");
            var res = newtype.GetMethod("go").Invoke(null, new object[] {0});
            //meth.Invoke(null, new object[0]);
            //lambda.DynamicInvoke(new object[0]);
            Console.WriteLine(" ");
        }

        [Test]
        public void TestZzz()
        {
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("foo"), AssemblyBuilderAccess.RunAndSave);

            var mod = asm.DefineDynamicModule("mymod", "tmp.dll", true);
            var type = mod.DefineType("baz", TypeAttributes.Public | TypeAttributes.Class);
            var meth = type.DefineMethod("go", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new [] {typeof(int)});

            var document = mod.DefineDocument("TestDebug2.txt", Guid.Empty, Guid.Empty, Guid.Empty);//Expression.SymbolDocument("TestDebug2.txt");

            meth.DefineParameter(1, ParameterAttributes.In, "$x");

            //var di = Expression.DebugInfo(sdi, 2, 2, 2, 13);

            //var exp = Expression.Divide(Expression.Constant(2), Expression.Subtract(Expression.Constant(4), Expression.Constant(4)));
            //var block = Expression.Block(di, exp);

            using(var il = new GroboIL(meth))
            {
//                var tst = Expression.Block(Expression.DebugInfo(sdi, 6, 20, 6, 27), Expression.Equal(variable, zero));
//                var iftrue = Expression.Block(Expression.DebugInfo(sdi, 10, 20, 10, 26), Expression.Add(variable, two));
//                var iffalse = Expression.Block(Expression.DebugInfo(sdi, 14, 20, 14, 26), Expression.Divide(variable, two));
//                var exp = Expression.Condition(tst, iftrue, iffalse);
//                var block = Expression.Block(Expression.DebugInfo(sdi, 4, 16, 15, 10), exp);

//                        nop                      // []
//        nop                      // []
//        ldarg.0                  // [Int32]
//        ldc.i4.0                 // [Int32, Int32]
//        ceq                      // [Int32]
//        brfalse ifFalse_5        // []
//        nop                      // []
//        ldarg.0                  // [Int32]
//        ldc.i4.2                 // [Int32, Int32]
//        add                      // [Int32]
//        br done_8                // [Int32]
//ifFalse_5:                       // []
//        nop                      // []
//        ldarg.0                  // [Int32]
//        ldc.i4.2                 // [Int32, Int32]
//        div                      // [Int32]
//done_8:                          // [Int32]
//        ret                      // []

                il.MarkSequencePoint(document, 3, 9, 3, 15);
                il.Nop();
                il.MarkSequencePoint(document, 4, 13, 4, 19);
                il.Nop();
                il.Ldarg(0);
                il.Ldc_I4(0);
                il.Ceq();
                var label = il.DefineLabel("ifFalse");
                il.Brfalse(label);
                il.MarkSequencePoint(document, 7, 13, 7, 19);
                il.Nop();
                il.Ldarg(0);
                il.Ldc_I4(2);
                il.Add();
                var doneLabel = il.DefineLabel("done");
                il.Br(doneLabel);
                il.MarkLabel(label);
                il.MarkSequencePoint(document, 10, 13, 10, 19);
                il.Nop();
                il.Ldarg(0);
                il.Ldc_I4(2);
                il.Mul();
                il.MarkLabel(doneLabel);
                il.MarkSequencePoint(document, 12, 5, 12, 6);
                il.Nop();
                il.Ret();
            }
            var newtype = type.CreateType();

            asm.Save("tmp.dll");
            newtype.GetMethod("go").Invoke(null, new object[]{0});
            //meth.Invoke(null, new object[0]);
            //lambda.DynamicInvoke(new object[0]);
            Console.WriteLine(" ");
        }

        [Test]
        public void Test()
        {
            if(asdfsadf())
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
        public void TestZzz2()
        {
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("foo"), AssemblyBuilderAccess.RunAndSave);

            var mod = asm.DefineDynamicModule("mymod", "tmp.dll", true);
            var type = mod.DefineType("baz", TypeAttributes.Public | TypeAttributes.Class);
            var meth = type.DefineMethod("go", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new [] {typeof(int)});

            var document = mod.DefineDocument("TestDebug2.txt", Guid.Empty, Guid.Empty, Guid.Empty);//Expression.SymbolDocument("TestDebug2.txt");

            //var di = Expression.DebugInfo(sdi, 2, 2, 2, 13);

            //var exp = Expression.Divide(Expression.Constant(2), Expression.Subtract(Expression.Constant(4), Expression.Constant(4)));
            //var block = Expression.Block(di, exp);

            using(var il = new GroboIL(meth))
            {
//                        nop                      // []
//        nop                      // []
//        ldc.i4.1                 // [Int32]
//        brfalse ifFalse_7        // []
//        nop                      // []
//        ldarg.0                  // [Int32]
//        br done_10               // [Int32]
//ifFalse_7:                       // []
//        nop                      // []
//        ldarg.0                  // [Int32]
//done_10:                         // [Int32]
//        stloc local_0            // []
//        nop                      // []
//        ldloc local_0            // [Int32]
//        ret                      // []


//                var tst = Expression.Block(Expression.DebugInfo(sdi, 6, 20, 6, 27), Expression.Constant(true));
//                var iftrue = Expression.Block(Expression.DebugInfo(sdi, 10, 20, 10, 26), variable);
//                var iffalse = Expression.Block(Expression.DebugInfo(sdi, 14, 20, 14, 26), variable);
//                var exp = Expression.Condition(tst, iftrue, iffalse);
//
//                /*
//                var returnTarget = Expression.Label(typeof(int));
//                var returnExpression = Expression.Return(returnTarget, exp, typeof(int));
//                var returnLabel = Expression.Label(returnTarget, Expression.Constant(0));
//                */
//
//                var block = Expression.Block(typeof(int), new[] { temp },
//                    Expression.DebugInfo(sdi, 4, 15, 4, 16), Expression.Assign(temp, exp), Expression.DebugInfo(sdi, 17, 16, 17, 21), temp);
//                //var block = Expression.Block(Expression.DebugInfo(sdi, 4, 16, 17, 10), Expression.Assign(temp, exp)));
//                var kek = Expression.Lambda(block, variable);

                il.MarkSequencePoint(document, 4, 15, 4, 16);
                il.Nop();
                il.Nop();
                il.Ldc_I4(1);
                il.MarkSequencePoint(document, 6, 20, 6, 27);
                var brFalse = il.DefineLabel("brFalse");
                il.Brfalse(brFalse);
                il.MarkSequencePoint(document, 10, 20, 10, 26);
                il.Nop();
                il.Ldarg(0);
                il.Ldc_I4(100);
                il.Add();
                var doneLabel = il.DefineLabel("done");
                il.Br(doneLabel);
                il.MarkSequencePoint(document, 14, 20, 14, 26);
                il.MarkLabel(brFalse);
                il.Nop();
                il.Ldarg(0);
                il.Ldc_I4(10);
                il.Add();
                il.MarkLabel(doneLabel);
                il.MarkSequencePoint(document, 16, 16, 16, 21);
                var local = il.DeclareLocal(typeof(int));
                il.Stloc(local);
                il.Nop();
                il.MarkSequencePoint(document, 17, 16, 17, 21);
                il.Ldloc(local);
                il.Ret();
            }
            var newtype = type.CreateType();

            asm.Save("tmp.dll");
            newtype.GetMethod("go").Invoke(null, new object[]{0});
            //meth.Invoke(null, new object[0]);
            //lambda.DynamicInvoke(new object[0]);
            Console.WriteLine(" ");
        }

        [Test, Ignore]
        public void TestDebugInfo2()
        {
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("foo"), AssemblyBuilderAccess.RunAndSave);

            var mod = asm.DefineDynamicModule("mymod", "tmp.dll", true);
            var type = mod.DefineType("baz", TypeAttributes.Public | TypeAttributes.Class);
            var meth = type.DefineMethod("go", MethodAttributes.Public | MethodAttributes.Static);

            var nestedType = type.DefineNestedType("qwerty", TypeAttributes.NestedPublic | TypeAttributes.Class);
            nestedType.DefineField("zzz", typeof(Guid), FieldAttributes.Static | FieldAttributes.Public);
            nestedType.CreateType();

            var document = mod.DefineDocument("TestDebug2.txt", Guid.Empty, Guid.Empty, Guid.Empty);//Expression.SymbolDocument("TestDebug2.txt");

            //var di = Expression.DebugInfo(sdi, 2, 2, 2, 13);

            //var exp = Expression.Divide(Expression.Constant(2), Expression.Subtract(Expression.Constant(4), Expression.Constant(4)));
            //var block = Expression.Block(di, exp);

            var il = meth.GetILGenerator();
            il.MarkSequencePoint(document, 2, 2, 2, 13);
            il.Emit(OpCodes.Ldc_I4_2);
            il.Emit(OpCodes.Ldc_I4_4);
            il.Emit(OpCodes.Ldc_I4_4);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Div);

            var newtype = type.CreateType();

            asm.Save("tmp.dll");
            newtype.GetMethod("go").Invoke(null, new object[0]);
            //meth.Invoke(null, new object[0]);
            //lambda.DynamicInvoke(new object[0]);
            Console.WriteLine(" ");
        }

        [Test, Ignore]
        public void TestDebugInfo3()
        {
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("foo"), AssemblyBuilderAccess.RunAndSave);

            var mod = asm.DefineDynamicModule("mymod", "tmp.dll", true);
            var type = mod.DefineType("baz", TypeAttributes.Public | TypeAttributes.Class);
            var meth = type.DefineMethod("go", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), new[] {typeof(IntPtr), typeof(int)});

            var nestedType = type.DefineNestedType("qwerty", TypeAttributes.NestedPublic | TypeAttributes.Class);
            nestedType.DefineField("zzz", typeof(Guid), FieldAttributes.Static | FieldAttributes.Public);
            nestedType.CreateType();

            var document = mod.DefineDocument("TestDebug2.txt", Guid.Empty, Guid.Empty, Guid.Empty);//Expression.SymbolDocument("TestDebug2.txt");

            //var di = Expression.DebugInfo(sdi, 2, 2, 2, 13);

            //var exp = Expression.Divide(Expression.Constant(2), Expression.Subtract(Expression.Constant(4), Expression.Constant(4)));
            //var block = Expression.Block(di, exp);

            var il = meth.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Unaligned, (byte)1);
            il.Emit(OpCodes.Stind_I4);
            il.Emit(OpCodes.Ret);

            var newtype = type.CreateType();

            asm.Save("tmp.dll");
            //newtype.GetMethod("go").Invoke(null, new object[0]);
            //meth.Invoke(null, new object[0]);
            //lambda.DynamicInvoke(new object[0]);
            Console.WriteLine(" ");
        }

        [Test, Ignore]
        public void TestDebug2()
        {
  // create a dynamic assembly and module 
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("foo"), AssemblyBuilderAccess.RunAndSave);

        ModuleBuilder module = assemblyBuilder.DefineDynamicModule("zzz", "HelloWorld.dll", true); // <-- pass 'true' to track debug info.

        // Tell Emit about the source file that we want to associate this with. 
        ISymbolDocumentWriter doc = module.DefineDocument("Source.txt", Guid.Empty, Guid.Empty, Guid.Empty);

        // create a new type to hold our Main method 
        TypeBuilder typeBuilder = module.DefineType("HelloWorldType", TypeAttributes.Public | TypeAttributes.Class);

        // create the Main(string[] args) method 
        MethodBuilder methodbuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public, typeof(int), new Type[] { typeof(string[]) });

        // generate the IL for the Main method 
        ILGenerator ilGenerator = methodbuilder.GetILGenerator();

        // Create a local variable of type 'string', and call it 'xyz'
        LocalBuilder localXYZ = ilGenerator.DeclareLocal(typeof(string));
        localXYZ.SetLocalSymInfo("xyz"); // Provide name for the debugger. 

        // Emit sequence point before the IL instructions. This is start line, start col, end line, end column, 

        // Line 2: xyz = "hello"; 
        ilGenerator.MarkSequencePoint(doc, 2, 1, 2, 100);
        ilGenerator.Emit(OpCodes.Ldstr, "Hello world!");
        ilGenerator.Emit(OpCodes.Stloc, localXYZ);

        // Line 3: Write(xyz); 
        MethodInfo infoWriteLine = typeof(System.Console).GetMethod("WriteLine", new Type[] { typeof(string) });
        ilGenerator.MarkSequencePoint(doc, 3, 1, 3, 100);
        ilGenerator.Emit(OpCodes.Ldloc, localXYZ);
        ilGenerator.EmitCall(OpCodes.Call, infoWriteLine, null);

        LocalBuilder localResult = ilGenerator.DeclareLocal(typeof(string));
        localResult.SetLocalSymInfo("result"); // Provide name for the debugger. 

        // Line 4: result = 0/0; 
        ilGenerator.MarkSequencePoint(doc, 4, 1, 4, 100);
        ilGenerator.Emit(OpCodes.Ldc_I4_0);
        ilGenerator.Emit(OpCodes.Ldc_I4_0);
        ilGenerator.Emit(OpCodes.Div);
        ilGenerator.Emit(OpCodes.Stloc, localResult);

        // Line 5: return result; 
        ilGenerator.MarkSequencePoint(doc, 5, 1, 5, 100);
        ilGenerator.Emit(OpCodes.Ldloc, localResult);
        ilGenerator.Emit(OpCodes.Ret);

        // bake it 
        Type helloWorldType = typeBuilder.CreateType();

        assemblyBuilder.Save("HelloWorld.dll");

        // This now calls the newly generated method. We can step into this and debug our emitted code!! 
        helloWorldType.GetMethod("Main").Invoke(null, new string[] { null }); // <-- step into        
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
            Assert.IsTrue(f(new TestStructA {S = "qzz", ArrayB = new[] {new TestStructB {S = "zzz"},}}));
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
            Assert.IsFalse(f(new TestStructA {ArrayB = new[] {new TestStructB {Y = 1},}}));
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
            Assert.AreEqual(1, f(a));
            Assert.AreEqual(0, f(null));
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
            Assert.IsTrue(f(new TestClassA {ArrayB = new[] {new TestClassB {S = "zzz"},}}));
        }

        [Test]
        public void TestSubLambda5()
        {
            Expression<Func<TestClassA, bool>> exp = a => a.ArrayB.Any(b => b.S == a.S && b.C.ArrayD.All(d => d.S == b.S && d.ArrayE.Any(e => e.S == a.S && e.S == b.S && e.S == d.S)));
            var f = Compile(exp, CompilerOptions.All);
            Assert.IsTrue(f(new TestClassA
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
            Assert.IsFalse(f(new TestClassA
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

        private void CompileAndSave<TDelegate>(Expression<TDelegate> lambda) where TDelegate : class
        {
            var da = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("dyn"), // call it whatever you want
                AssemblyBuilderAccess.Save);

            var dm = da.DefineDynamicModule("dyn_mod", "dyn.dll");
            var dt = dm.DefineType("dyn_type");
            var method = dt.DefineMethod("Foo", MethodAttributes.Public | MethodAttributes.Static, lambda.ReturnType, lambda.Parameters.Select(parameter => parameter.Type).ToArray());

            //lambda.CompileToMethod(method);
            LambdaCompiler.CompileToMethod(lambda, method, CompilerOptions.All);
            dt.CreateType();

            da.Save("dyn.dll");
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
    }
}