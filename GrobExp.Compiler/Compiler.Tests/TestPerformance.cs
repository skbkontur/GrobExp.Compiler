using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using GrEmit;

using GrobExp.Compiler;

using NUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class TestPerformance
    {
        public string zzz(int x)
        {
            switch(x)
            {
            case 0:
                return "zzz";
            case 3:
                return "qzz";
            case 6:
                return "qxx";
            case 9:
                return "jgjkfgfhgf";
            default:
                return "xxx";
            }
        }

        public string qzz(string s)
        {
            switch(s)
            {
            case null:
            case "0":
            case "2":
                return "zzz";
            case "5":
            case "1000001":
                return "qxx";
            case "7":
            case "1000000":
                return "qzz";
            default:
                return "xxx";
            }
        }

        public static int? Zzz(object x)
        {
            if(x is int)
                return (int)x;
            return null;
        }

        static int Add(int x, double y) { return (int)(x + y); }

        [Test, Ignore]
        public unsafe void TestWriteAssemblerCode()
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(object), new[] {typeof(object)}, typeof(string), true);
            using(var il = new GroboIL(method, false))
            {
                il.Ldarg(0);
                il.Isinst(typeof(int?));
                il.Ret();
            }
            method.CreateDelegate(typeof(Func<object, object>));
            var pointer = dynamicMethodPointerExtractor(method);
            var b = (byte*)pointer;
            //var zzz = Convert.FromBase64String(@"VldVU0FSTInOSItsJFhIi3wkYEGJyonRRInDRInQVQ8oBg8oZhAPKMgPKOwPKNAPKPQPKNgPKPwPWUUAD1llEA9ZTTAPWW1AD1lVYA9ZdXAPWZ2QAAAAD1m9oAAAAPIPfMHyD3zl8g980/IPfPfyD3zC8g985g9Y4A8oRiAPKMgPKNAPKNgPWUUgD1lNUA9ZlYAAAAAPWZ2wAAAA8g98wfIPfNPyD3zCD1jgD1ilwAAAAA8rJ0iBxdAAAABIg8cQ/8gPhVf///9Ig8YwXf/LD4VG////Z0ONBFJnQY0EgsHgBEiYSAHF/8kPhSn///9BWltdX17D");
            var zzz = Convert.FromBase64String(@"VldVU0FSTInOSItsJFBIi3wkWEGJyonRRInDRInQVQ8oBg8oZhAPKMgPKOwPKNAPKPQPKNgPKPwPWUUAD1llEA9ZTTAPWW1AD1lVYA9ZdXAPWZ2QAAAAD1m9oAAAAPIPfMHyD3zl8g980/IPfPfyD3zC8g985g9Y4A8oRiAPKMgPKNAPKNgPWUUgD1lNUA9ZlYAAAAAPWZ2wAAAA8g98wfIPfNPyD3zCD1jgD1ilwAAAAA8rJ0iBxdAAAABIg8cQ/8gPhVf///9Ig8YwXf/LD4VG////Z0ONBFJnQY0EgsHgBEiYSAHF/8kPhSn///9BWltdX17D");
            fixed(byte* z = &zzz[0])
            {
                b = z;
                for(int i = 0; i < 20; ++i)
                {
                    for(int j = 0; j < 10; ++j)
                        Console.Write(string.Format("{0:X2} ", *b++));
                    Console.WriteLine();
                }
            }
        }

        [Test, Ignore]
        public unsafe void TestWriteAssemblerCode0()
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(int), new[] {typeof(double)}, typeof(string), true);
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0);
                il.Conv<int>();
                il.Ret();
            }
            var func = (Func<double, int>)method.CreateDelegate(typeof(Func<double, int>));
            Console.WriteLine(func(3000000000));
            var pointer = dynamicMethodPointerExtractor(method);
            var b = (byte*)pointer;
            for(int i = 0; i < 20; ++i)
            {
                for(int j = 0; j < 10; ++j)
                    Console.Write(string.Format("{0:X2} ", *b++));
                Console.WriteLine();
            }
        }

        [Test, Ignore]
        public unsafe void TestWriteAssemblerCode2()
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(IntPtr), Type.EmptyTypes, typeof(string), true);
            var il = method.GetILGenerator();
            var ptr = new IntPtr(0x40100C);
            if(IntPtr.Size == 4)
                il.Emit(OpCodes.Ldc_I4, ptr.ToInt32());
            else il.Emit(OpCodes.Ldc_I8, ptr.ToInt64());
            il.Emit(OpCodes.Conv_U);
            il.EmitCalli(OpCodes.Calli, CallingConventions.Standard, typeof(void), Type.EmptyTypes, null);
            il.Emit(OpCodes.Ldnull);

            il.Emit(OpCodes.Ret);
            
            method.CreateDelegate(typeof(Func<IntPtr>));
            var pointer = dynamicMethodPointerExtractor(method);
            var b = (byte*)pointer;
            for(int i = 0; i < 20; ++i)
            {
                for(int j = 0; j < 10; ++j)
                    Console.Write(string.Format("{0:X2} ", *b++));
                Console.WriteLine();
            }
        }

        [Test, Ignore]
        public unsafe void TestWriteAssemblerCode3()
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(void), new[] { typeof(IntPtr), typeof(int) }, typeof(string), true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            if(IntPtr.Size == 8)
                il.Emit(OpCodes.Ldc_I8, 0x123456789ABCDEF1);
            else
                il.Emit(OpCodes.Ldc_I4, 0x12345678);
            il.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, typeof(void), new[] {typeof(IntPtr), typeof(int)});
            il.Emit(OpCodes.Ret);
            method.CreateDelegate(typeof(Action<IntPtr, int>));
            var pointer = dynamicMethodPointerExtractor(method);
            var b = (byte*)pointer;
            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 10; ++j)
                    Console.Write(string.Format("{0:X2} ", *b++));
                Console.WriteLine();
            }
            Console.WriteLine(TestStind_i4(123456678)[1]);
        }

        [Test, Ignore]
        public unsafe void TestWriteAssemblerCode4()
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(int), new[] { typeof(int), typeof(int), typeof(int), typeof(int) }, typeof(string), true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            byte[] marker;
            if(IntPtr.Size == 8)
            {
                const long addr = 0x123456789ABCDEF1;
                marker = BitConverter.GetBytes(addr);
                il.Emit(OpCodes.Ldc_I8, addr);
            }
            else
            {
                const int addr = 0x12345678;
                marker = BitConverter.GetBytes(addr);
                il.Emit(OpCodes.Ldc_I4, addr);
            }
            il.EmitCalli(OpCodes.Calli, CallingConventions.Standard, typeof(int), new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), }, null);
            il.Emit(OpCodes.Ret);
            var func = (Func<int, int, int, int, int>)method.CreateDelegate(typeof(Func<int, int, int, int, int>));
            var pointer = dynamicMethodPointerExtractor(method);

            Console.WriteLine("{0:X}", pointer.ToInt64());

            //Replace((byte*)pointer, marker);

            var b = (byte*)pointer;
            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 10; ++j)
                    Console.Write(string.Format("{0:X2} ", *b++));
                Console.WriteLine();
            }

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 1000000001; ++i)
            {
                func(i, i, i, i);
            }
            var elapsed = stopwatch.Elapsed;
            Console.WriteLine(elapsed.TotalMilliseconds);
        }

        private unsafe void Replace(byte* ptr, byte[] marker)
        {
            for(;;++ptr)
            {
                if(marker.Where((t, i) => *(ptr + i) != t).Any()) continue;
                int prevJunk = (IntPtr.Size == 4 ? 1 : 2);
                int nextJunk = 2;
                ptr -= prevJunk;
                byte[] body;
                if (IntPtr.Size == 4)
                {
                    // x86
                    body = new byte[]
                    {
                        0x31, 0xC0, // xor eax, eax
                    };
                }
                else
                {
                    body = new byte[]
                    {
                        0x48, 0x31, 0xC0, // xor rax, rax
                    };
                }
                foreach(byte b in body)
                    *ptr++ = b;
                for(int i = 0; i < prevJunk + marker.Length + nextJunk - body.Length; ++i)
                    *ptr++ = 0x90; // nop
                break;
            }
        }

        [Flags]
        private enum AllocationTypes : uint
        {
            Commit = 0x1000, Reserve = 0x2000,
            Reset = 0x80000, LargePages = 0x20000000,
            Physical = 0x400000, TopDown = 0x100000,
            WriteWatch = 0x200000
        }

        [Flags]
        private enum MemoryProtections : uint
        {
            Execute = 0x10, ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40, ExecuteWriteCopy = 0x80,
            NoAccess = 0x01, ReadOnly = 0x02,
            ReadWrite = 0x04, WriteCopy = 0x08,
            GuartModifierflag = 0x100, NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [Flags]
        private enum FreeTypes : uint
        {
            Decommit = 0x4000, Release = 0x8000
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private unsafe delegate int ZzzUnmanagedDelegate(int x, int y);

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr VirtualAlloc(
                IntPtr lpAddress,
                UIntPtr dwSize,
                AllocationTypes flAllocationType,
                MemoryProtections flProtect);

            [DllImport("kernel32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool VirtualFree(
                IntPtr lpAddress,
                uint dwSize,
                FreeTypes flFreeType);
        }

        [Test]
        public unsafe void TestMarshal()
        {
            byte[] body;
            if (IntPtr.Size == 4)
            {
                // x86
                /*
                 * xor eax, eax // 0x31, 0xC0
                 * ret 8 // 0xC2, 0x08, 0x00
                 */
                body = new byte[]
                    {
                        0x31, 0xC0, // xor eax, eax
                        0xC2, 0x08, 0x00 // ret 8
                    };
            }
            else
            {
                body = new byte[]
                    {
                        0x48, 0x31, 0xC0, // xor rax, rax
                        0xC3 // ret
                    };
            }
            IntPtr p = NativeMethods.VirtualAlloc(
                    IntPtr.Zero,
                    new UIntPtr((uint)body.Length),
                    AllocationTypes.Commit | AllocationTypes.Reserve,
                    MemoryProtections.ExecuteReadWrite);
            Marshal.Copy(body, 0, p, body.Length);
            var func = (ZzzUnmanagedDelegate)Marshal.GetDelegateForFunctionPointer(p, typeof(ZzzUnmanagedDelegate));
            var method = ((MulticastDelegate)func).Method;
            var methodHandle = (RuntimeMethodHandle)method.GetType().GetProperty("MethodHandle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetGetMethod().Invoke(method, new object[0]);
            var pointer = methodHandle.GetFunctionPointer();
            var b = (byte*)pointer;
            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 10; ++j)
                    Console.Write(string.Format("{0:X2} ", *b++));
                Console.WriteLine();
            }

            func(0, 1);

//            var stopwatch = Stopwatch.StartNew();
//            for (int i = 0; i < 1000000001; ++i)
//            {
//                func(i, i);
//            }
//            var elapsed = stopwatch.Elapsed;
//            Console.WriteLine(elapsed.TotalMilliseconds);
        }

        public unsafe byte[] TestStind_i4(int x)
        {
            var arr = new byte[10];
            fixed(byte* a = &arr[0])
            {
                *(int*)(a + 1) = x;
                *(int*)(a + 2) = x;
                *(int*)(a + 3) = x;
                *(int*)(a + 4) = x;
                *(int*)(a + 5) = x;
                *(int*)(a + 6) = x;
                *(int*)(a + 7) = x;
            }
            return arr;
        }

        public class Node
        {
            public readonly Dictionary<char, Node> jumps = new Dictionary<char, Node>();
            public bool accepted;
        }

        public bool Match(Node start, string str)
        {
            var node = start;
            for(int i = 0; i < str.Length; ++i)
            {
                Node jump;
                if(!node.jumps.TryGetValue(str[i], out jump))
                    return false;
                node = jump;
            }
            return node.accepted;
        }

        public abstract class Node2
        {
            public abstract Node2 Jump(char c);
            public bool accepted;
        }

        public class Node2_09 : Node2
        {
            public Node2 child;

            public override Node2 Jump(char c)
            {
                if(c >= '0' && c <= '9') return child;
                return null;
            }
        }

        public class Node2_pm09 : Node2
        {
            public Node2 child_pm;
            public Node2 child_09;

            public override Node2 Jump(char c)
            {
                if(c == '+' || c == '-')
                    return child_pm;
                if(c >= '0' && c <= '9')
                    return child_09;
                return null;
            }
        }

        public class Node2_dot09 : Node2
        {
            public Node2 child_dot;
            public Node2 child_09;

            public override Node2 Jump(char c)
            {
                if(c == '.')
                    return child_dot;
                if(c >= '0' && c <= '9')
                    return child_09;
                return null;
            }
        }

        public bool Match3(Node2 start, string str)
        {
            var node = start;
            for (int i = 0; i < str.Length; ++i)
            {
                node = node.Jump(str[i]);
                if(node == null)
                    return false;
            }
            return node.accepted;
        }

        public bool Match2(string str)
        {
            int idx = 0;
            int len = str.Length;
            char c;

        _1: if (idx >= len) return false;
            c = str[idx++];
            if (c == '+' || c == '-') goto _2;
            if (c >= '0' && c <= '9') goto _3;
            return false;

        _2: if (idx >= len) return false;
            c = str[idx++];
            if (c >= '0' && c <= '9') goto _3;
            return false;

        _3: if (idx >= len) return true;
            c = str[idx++];
            if (c >= '0' && c <= '9') goto _3;
            if (c == '.') goto _4;
            return false;

        _4: if (idx >= len) return true;
            c = str[idx++];
            if (c >= '0' && c <= '9') goto _4;
            return false;
        }

        public static void Main(string[] args)
        {
            var test = new TestPerformance();
            test.TestAutomaton();
        }

        [Test, Ignore]
        public void TestAutomaton()
        {
            var s1 = new Node();
            var s2 = new Node();
            var s3 = new Node();
            var s4 = new Node();
            s1.jumps.Add('+', s2);
            s1.jumps.Add('-', s2);
            for(char c = '0'; c <= '9'; ++c)
                s1.jumps.Add(c, s3);
            for(char c = '0'; c <= '9'; ++c)
                s2.jumps.Add(c, s3);
            s3.jumps.Add('.', s4);
            for(char c = '0'; c <= '9'; ++c)
                s3.jumps.Add(c, s3);
            for(char c = '0'; c <= '9'; ++c)
                s4.jumps.Add(c, s4);
            s3.accepted = true;
            s4.accepted = true;

            var t1 = new Node2_pm09();
            var t2 = new Node2_09();
            var t3 = new Node2_dot09();
            var t4 = new Node2_09();
            t1.child_pm = t2;
            t1.child_09 = t3;
            t2.child = t3;
            t3.child_dot = t4;
            t3.child_09 = t3;
            t4.child = t4;
            t3.accepted = true;
            t4.accepted = true;

            Func<string, bool> func = s => Match(s1, s);
            Func<string, bool> func2 = Match2;
            Func<string, bool> func3 = s => Match3(t1, s);

            var regex = new Regex(@"^[\-+]?[0-9]+(\.[0-9]*)?$");
            Func<string, bool> func4 = regex.IsMatch;
            var regex2 = new Regex(@"^[\-+]?[0-9]+(\.[0-9]*)?$", RegexOptions.Compiled);
            Func<string, bool> func5 = regex2.IsMatch;

            Console.WriteLine(func("123456789.12345678.9"));
            Console.WriteLine(func2("123456789.12345678.9"));
            Console.WriteLine(func3("123456789.12345678.9"));
            Console.WriteLine(func4("123456789.12345678.9"));
            Console.WriteLine(func5("123456789.12345678.9"));
            Console.WriteLine("Compiled automaton");
            var ethalon = MeasureSpeed(func2, "123456789.123456789", 10000001, null);
            Console.WriteLine("Automaton");
            MeasureSpeed(func, "123456789.123456789", 10000001, ethalon);
            Console.WriteLine("Optimized automaton");
            MeasureSpeed(func3, "123456789.123456789", 10000001, ethalon);
            Console.WriteLine("regex");
            MeasureSpeed(func4, "123456789.123456789", 10000001, ethalon);
            Console.WriteLine("regex compiled");
            MeasureSpeed(func5, "123456789.123456789", 10000001, ethalon);
        }

        private class ParsedXSD
        {
            public int? Length;
            public int? MinLength;
            public int? MaxLength;
            public string MinInclusive;
            public string MinExclusive;
            public string MaxInclusive;
            public string MaxExclusive;
            public int? TotalDigits;
            public int? FractionDigits;
            public string Pattern;
            public List<string> Enumeration;

            public bool Check1(string str)
            {
                if (str.Length != Length.Value)
                    return false;
                return true;
            }

            public bool Check2(string str)
            {
                if (str.Length < MinLength.Value)
                    return false;
                if (str.Length < MaxLength.Value)
                    return false;
                return true;
            }

            public bool Check(string str)
            {
                if (Length != null)
                {
                    if (str.Length != Length.Value)
                        return false;
                }
                if (MinLength != null)
                {
                    if (str.Length < MinLength.Value)
                        return false;
                }
                if (MaxLength != null)
                {
                    if (str.Length < MaxLength.Value)
                        return false;
                }
                if (MinInclusive != null)
                {
                    if (str != MinInclusive)
                        return false;
                }
                if (MinExclusive != null)
                {
                    if (str != MinExclusive)
                        return false;
                }
                if (MaxInclusive != null)
                {
                    if (str != MaxInclusive)
                        return false;
                }
                if (MaxExclusive != null)
                {
                    if (str != MaxExclusive)
                        return false;
                }
                if (TotalDigits != null)
                {
                    if (str.Length != TotalDigits.Value)
                        return false;
                }
                if (FractionDigits != null)
                {
                    if (str.Length != FractionDigits.Value)
                        return false;
                }
                if (Pattern != null)
                {
                    if (str != Pattern)
                        return false;
                }
                if (Enumeration != null)
                {
                    if (str.Length != Enumeration.Count)
                        return false;
                }
                return true;
            }
        }

        [Test]
        public void Test_XSD()
        {
            var parsedXsd1 = new ParsedXSD
                {
                    Length = 5
                };
            var parsedXsd2 = new ParsedXSD
                {
                    MinLength = 5,
                    MaxLength = 10
                };
            Func<string, bool> func = parsedXsd1.Check;
            Func<string, bool> func1 = parsedXsd1.Check1;
            Func<string, bool> func2 = parsedXsd2.Check2;
            Console.WriteLine("Check");
            var ethalon = MeasureSpeed(func, "1786348726", 1000000001, null);
            Console.WriteLine("Check2");
            MeasureSpeed(func2, "2135423542", 1000000001, ethalon);
            Console.WriteLine("Check1");
            MeasureSpeed(func1, "2345234533", 1000000001, ethalon);
        }

        [Test]
        public void Test_Gen()
        {
            var output = new StringBuilder();
            output.AppendLine("10 20 2");
            output.AppendLine("9 1 3");
            output.AppendLine("preflop");
            for(int i = 1; i <= 9; ++i)
                output.AppendLine(string.Format("Player{0} 20000", i));
            int n = 110;
            int[] bets = new int[9];
            bets[1] = 10;
            bets[2] = 20;
            var actions = 8 + 9 * n;
            output.AppendLine(string.Format("{0}", actions));
            int k = 4;
            for(int i = 0; i < actions; ++i)
            {
                output.AppendLine(string.Format("Player{0} raises 20 to {1}", k, 20 * (i + 2)));
                bets[k - 1] = 20 * (i + 2);
                k = 1 + (k - 1 + 1) % 9;
            }
            for(int i = 0; i < 9; ++i)
                output.Append(string.Format("{0} ", 20000 - 2 - bets[i]));
            System.IO.File.WriteAllText(@"c:\temp\big.in", output.ToString());
        }

        [Test, Ignore]
        public void TestSimple()
        {
            Expression<Func<TestClassA, int?>> exp = o => o.ArrayB[0].C.ArrayD[0].X;
            var a = new TestClassA {ArrayB = new TestClassB[1] {new TestClassB {C = new TestClassC {ArrayD = new TestClassD[1] {new TestClassD {X = 5}}}}}};
            Console.WriteLine("Sharp");
            var ethalon = MeasureSpeed(Func1, a, 1000000000, null);
            Console.WriteLine("GroboCompile without checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.None), a, 1000000000, ethalon);
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), a, 1000000000, ethalon);
            Console.WriteLine("Compile");
            MeasureSpeed(exp.Compile(), a, 100000000, ethalon);
        }

        [Test, Ignore]
        public void TestSubLambda1()
        {
            Expression<Func<TestClassA, bool>> exp = o => o.ArrayB.Any(b => b.S == o.S);
            var a = new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB {S = "zzz"},}};
            Console.WriteLine("Sharp");
            var ethalon = MeasureSpeed(Func2, a, 100000000, null);
            Console.WriteLine("GroboCompile without checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.None), a, 100000000, ethalon);
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), a, 100000000, ethalon);
            Console.WriteLine("Compile");
            MeasureSpeed(exp.Compile(), a, 1000000, ethalon);
        }

        [Test, Ignore]
        public void TestSubLambda1WithGarbageCollecting()
        {
            Expression<Func<TestClassA, bool>> exp = o => o.ArrayB.Any(b => b.S == o.S);
            var a = new TestClassA {S = "zzz", ArrayB = new[] {new TestClassB {S = "zzz"},}};
            Console.WriteLine("GroboCompile without checking");
            stop = false;
            var thread = new Thread(Collect);
            thread.Start();
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.None), a, 100000000, null);
            stop = true;
        }

        [Test, Ignore]
        public void TestSubLambda2()
        {
            Expression<Func<TestClassA, bool>> exp = o => o.ArrayB.Any(b => b.S == o.S && b.C.ArrayD.All(d => d.S == b.S && d.ArrayE.Any(e => e.S == o.S && e.S == b.S && e.S == d.S)));
            var a = new TestClassA
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
                };
            Console.WriteLine("Sharp");
            var ethalon = MeasureSpeed(Func3, a, 100000000, null);
            Console.WriteLine("GroboCompile without checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.None), a, 100000000, ethalon);
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), a, 100000000, ethalon);
            Console.WriteLine("Compile");
            MeasureSpeed(exp.Compile(), a, 1000000, ethalon);
        }

        [Test, Ignore]
        public void TestInvoke1()
        {
            Expression<Func<TestClassA, int>> exp = o => func(o.Y, o.Z);
            var a = new TestClassA {Y = 1, Z = 2};
            Console.WriteLine("Compile");
            var ethalon = MeasureSpeed(exp.Compile(), a, 100000000, null);
            Console.WriteLine("Sharp");
            MeasureSpeed(Func4, a, 100000000, ethalon);
            Console.WriteLine("GroboCompile without checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.None), a, 100000000, ethalon);
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), a, 100000000, ethalon);
        }

        [Test, Ignore]
        public void TestInvoke2()
        {
            Expression<Func<int, int, int>> lambda = (x, y) => x + y;
            ParameterExpression parameter = Expression.Parameter(typeof(TestClassA));
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(Expression.Invoke(lambda, Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("Y")), Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("Z"))), parameter);
            var a = new TestClassA {Y = 1, Z = 2};
            Console.WriteLine("Sharp");
            var ethalon = MeasureSpeed(Func5, a, 100000000, null);
            Console.WriteLine("GroboCompile without checking");
            Func<TestClassA, int> compile1 = LambdaCompiler.Compile(exp, CompilerOptions.None);
            MeasureSpeed(compile1, a, 100000000, ethalon);
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), a, 100000000, ethalon);
            Console.WriteLine("Compile");
            Func<TestClassA, int> compile = exp.Compile();
            MeasureSpeed(compile, a, 100000000, ethalon);
            Console.WriteLine("Build1");
            Func<TestClassA, int> build1 = Build1();
            MeasureSpeed(build1, a, 100000000, ethalon);
            Console.WriteLine("Build2");
            Func<TestClassA, int> build2 = Build1();
            MeasureSpeed(build2, a, 100000000, ethalon);
        }

        [Test, Ignore]
        public void TestInvoke3()
        {
            Expression<Func<int, int, int>> sum = (x, y) => x + y;
            Expression<Func<int, int, int>> mul = (x, y) => x * y;
            ParameterExpression parameter = Expression.Parameter(typeof(TestClassA));
            Expression<Func<TestClassA, int>> exp = Expression.Lambda<Func<TestClassA, int>>(Expression.Invoke(sum, Expression.Invoke(mul, Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("Y")), Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("Z"))), Expression.Invoke(mul, Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("P")), Expression.MakeMemberAccess(parameter, typeof(TestClassA).GetField("Q")))), parameter);
            var a = new TestClassA {Y = 1, Z = 2, P = 3, Q = 4};
            Console.WriteLine("Sharp");
            var ethalon = MeasureSpeed(Func7, a, 100000000, null);
            Console.WriteLine("GroboCompile without checking");
            Func<TestClassA, int> compile1 = LambdaCompiler.Compile(exp, CompilerOptions.None);
            MeasureSpeed(compile1, a, 100000000, ethalon);
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), a, 100000000, ethalon);
            Console.WriteLine("Compile");
            Func<TestClassA, int> compile = exp.Compile();
            MeasureSpeed(compile, a, 100000000, ethalon);
//            Console.WriteLine("Build1");
//            Func<TestClassA, int> build1 = Build1();
//            MeasureSpeed(build1, a, 100000000, ethalon);
//            Console.WriteLine("Build2");
//            Func<TestClassA, int> build2 = Build1();
//            MeasureSpeed(build2, a, 100000000, ethalon);
        }

        [Test, Ignore]
        public void TestFactorial()
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
            Console.WriteLine("Sharp");

            var ethalon = MeasureSpeed(Func6, 5, 100000000, null);
            Console.WriteLine("GroboCompile without checking");
            Func<int, int> compile1 = LambdaCompiler.Compile(exp, CompilerOptions.None);
            MeasureSpeed(compile1, 5, 100000000, ethalon);
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), 5, 100000000, ethalon);
            Console.WriteLine("Compile");
            Func<int, int> compile = exp.Compile();
            MeasureSpeed(compile, 5, 100000000, ethalon);
        }

        [Test, Ignore]
        public void TestSwitch1()
        {
            Console.WriteLine("Sharp");
            var ethalon = MeasureSpeed(Func7, 2, 1000000000, null);
            var func1 = BuildSwitch1();
            Console.WriteLine("Switch1");
            MeasureSpeed(func1, 2, 1000000000, ethalon);
            var func2 = BuildSwitch2();
            Console.WriteLine("Switch2");
            MeasureSpeed(func2, 2, 1000000000, ethalon);

            ParameterExpression a = Expression.Parameter(typeof(int));
            var exp = Expression.Lambda<Func<int, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant(0), Expression.Constant(2)),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant(5), Expression.Constant(1000001)),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant(7), Expression.Constant(1000000))
                    ),
                a
                );
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), 2, 1000000000, ethalon);
            Console.WriteLine("GroboCompile without checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.None), 2, 1000000000, ethalon);
            Console.WriteLine("Compile");
            MeasureSpeed(exp.Compile(), 2, 1000000000, ethalon);
        }

        [Test, Ignore]
        public void TestSwitch2()
        {
            Console.WriteLine("Sharp");
            var ethalon = MeasureSpeed(Func8, "1000000", 100000000, null);
            var func4 = BuildSwitch4();
            Console.WriteLine("Switch4");
            MeasureSpeed(func4, "1000000", 100000000, ethalon);

            ParameterExpression a = Expression.Parameter(typeof(string));
            var exp = Expression.Lambda<Func<string, string>>(
                Expression.Switch(
                    a,
                    Expression.Constant("xxx"),
                    Expression.SwitchCase(Expression.Constant("zzz"), Expression.Constant("0"), Expression.Constant("2")),
                    Expression.SwitchCase(Expression.Constant("qxx"), Expression.Constant("5"), Expression.Constant("1000001")),
                    Expression.SwitchCase(Expression.Constant("qzz"), Expression.Constant("7"), Expression.Constant("1000000"))
                    ),
                a
                );
            Console.WriteLine("GroboCompile with checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.All), "1000000", 100000000, ethalon);
            Console.WriteLine("GroboCompile without checking");
            MeasureSpeed(LambdaCompiler.Compile(exp, CompilerOptions.None), "1000000", 100000000, ethalon);
            Console.WriteLine("Compile");
            MeasureSpeed(exp.Compile(), "1000000", 100000000, ethalon);
        }

        [Test, Ignore]
        public void TestCalls()
        {
            var test = (ITest)new TestImpl();
            Console.WriteLine("Pure call");
            MeasureSpeed(test, 100000000);
            Console.WriteLine(x);
            test = BuildCall();
            Console.WriteLine("Call");
            MeasureSpeed(test, 100000000);
            Console.WriteLine(x);
            test = BuildDelegate();
            Console.WriteLine("Dynamic method through delegate");
            MeasureSpeed(test, 100000000);
            Console.WriteLine(x);
            test = BuildCalli();
            Console.WriteLine("Dynamic method through calli");
            MeasureSpeed(test, 100000000);
            Console.WriteLine(x);
        }

        [Test, Ignore]
        public void TestCalliWithGarbageCollecting()
        {
            stop = false;
            var thread = new Thread(Collect);
            thread.Start();

            var test = BuildCalli();
            Console.WriteLine("Dynamic method through calli");
            MeasureSpeed(test, 1000000000);
            Console.WriteLine(x);

            stop = true;
        }

        public static int x;
        public static readonly AssemblyBuilder Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
        public static readonly ModuleBuilder Module = Assembly.DefineDynamicModule(Guid.NewGuid().ToString());

        public class TestImpl : ITest
        {
            public void DoNothing()
            {
                DoNothingImpl();
            }

            private void DoNothingImpl()
            {
                ++x;
            }
        }

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
            public int P;
            public int Q;
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

        public interface ITest
        {
            void DoNothing();
        }

        private void Collect()
        {
            while(!stop)
            {
                Thread.Sleep(100);
                GC.Collect();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int? Func1(TestClassA a)
        {
            return a.ArrayB[0].C.ArrayD[0].X;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool Func2(TestClassA a)
        {
            return a.ArrayB.Any(b => b.S == a.S);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool Func3(TestClassA a)
        {
            return a.ArrayB.Any(b => b.S == a.S && b.C.ArrayD.All(d => d.S == b.S && d.ArrayE.Any(e => e.S == a.S && e.S == b.S && e.S == d.S)));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int Func4(TestClassA a)
        {
            return xfunc(a.Y, a.Z);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int Func5(TestClassA a)
        {
            return a.Y + a.Z;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int Func6(int n)
        {
            var result = 1;
            while(true)
            {
                if(n > 1)
                    result *= n--;
                else break;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int Func7(TestClassA a)
        {
            return a.Y * a.Z + a.P * a.Q;
        }

        private Func<TestClassA, int> Build1()
        {
            var typeBuilder = Module.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class | TypeAttributes.Public);
            var method = typeBuilder.DefineMethod("zzz", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new[] {typeof(TestClassA)});
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0);
                il.Ldfld(typeof(TestClassA).GetField("Y"));
                var y = il.DeclareLocal(typeof(int));
                il.Stloc(y);
                il.Ldarg(0);
                il.Ldfld(typeof(TestClassA).GetField("Z"));
                var z = il.DeclareLocal(typeof(int));
                il.Stloc(z);
                il.Ldloc(y);
                il.Ldloc(z);
                il.Add();
                il.Ret();
            }
            var type = typeBuilder.CreateType();
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(Func<TestClassA, int>), Type.EmptyTypes, Module, true);
            using(var il = new GroboIL(dynamicMethod))
            {
                il.Ldnull();
                il.Ldftn(type.GetMethod("zzz"));
                il.Newobj(typeof(Func<TestClassA, int>).GetConstructor(new[] {typeof(object), typeof(IntPtr)}));
                il.Ret();
            }
            return ((Func<Func<TestClassA, int>>)dynamicMethod.CreateDelegate(typeof(Func<Func<TestClassA, int>>)))();
        }

        private Func<TestClassA, int> Build2()
        {
            var typeBuilder = Module.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class | TypeAttributes.Public);
            var method = typeBuilder.DefineMethod("zzz", MethodAttributes.Public, typeof(int), new[] {typeof(TestClassA)});
            using(var il = new GroboIL(method))
            {
                il.Ldarg(1);
                il.Ldfld(typeof(TestClassA).GetField("Y"));
                il.Ldarg(1);
                il.Ldfld(typeof(TestClassA).GetField("Z"));
                var y = il.DeclareLocal(typeof(int));
                var z = il.DeclareLocal(typeof(int));
                il.Stloc(z);
                il.Stloc(y);
                il.Ldloc(y);
                il.Ldloc(z);
                il.Add();
                il.Ret();
            }
            var type = typeBuilder.CreateType();
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(Func<TestClassA, int>), new[] {typeof(object)}, Module, true);
            using(var il = new GroboIL(dynamicMethod))
            {
                il.Ldarg(0);
                il.Ldftn(type.GetMethod("zzz"));
                il.Newobj(typeof(Func<TestClassA, int>).GetConstructor(new[] {typeof(object), typeof(IntPtr)}));
                il.Ret();
            }
            return ((Func<object, Func<TestClassA, int>>)dynamicMethod.CreateDelegate(typeof(Func<object, Func<TestClassA, int>>)))(Activator.CreateInstance(type));
        }

        private ITest BuildCall()
        {
            var typeBuilder = Module.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class | TypeAttributes.Public);
            var doNothingMethod = typeBuilder.DefineMethod("DoNothingImpl", MethodAttributes.Public, typeof(void), Type.EmptyTypes);
            using(var il = new GroboIL(doNothingMethod))
            {
                il.Ldfld(xField);
                il.Ldc_I4(1);
                il.Add();
                il.Stfld(xField);
                il.Ret();
            }
            var method = typeBuilder.DefineMethod("DoNothing", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), Type.EmptyTypes);
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0);
                il.Call(doNothingMethod);
                il.Ret();
            }
            typeBuilder.DefineMethodOverride(method, typeof(ITest).GetMethod("DoNothing"));
            typeBuilder.AddInterfaceImplementation(typeof(ITest));
            var type = typeBuilder.CreateType();
            return (ITest)Activator.CreateInstance(type);
        }

        private ITest BuildDelegate()
        {
            var action = Build();
            var typeBuilder = Module.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class | TypeAttributes.Public);
            var actionField = typeBuilder.DefineField("action", typeof(Action), FieldAttributes.Private | FieldAttributes.InitOnly);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new[] {typeof(Action)});
            using(var il = new GroboIL(constructor))
            {
                il.Ldarg(0);
                il.Ldarg(1);
                il.Stfld(actionField);
                il.Ret();
            }
            var method = typeBuilder.DefineMethod("DoNothing", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), Type.EmptyTypes);
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0);
                il.Ldfld(actionField);
                il.Call(typeof(Action).GetMethod("Invoke", Type.EmptyTypes), typeof(Action));
                il.Ret();
            }
            typeBuilder.DefineMethodOverride(method, typeof(ITest).GetMethod("DoNothing"));
            typeBuilder.AddInterfaceImplementation(typeof(ITest));
            var type = typeBuilder.CreateType();
            return (ITest)Activator.CreateInstance(type, new object[] {action.Item1});
        }

        private ITest BuildCalli()
        {
            var action = Build();
            var typeBuilder = Module.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class | TypeAttributes.Public);
            var pointerField = typeBuilder.DefineField("pointer", typeof(IntPtr), FieldAttributes.Private | FieldAttributes.InitOnly);
            var delegateField = typeBuilder.DefineField("delegate", typeof(Delegate), FieldAttributes.Private | FieldAttributes.InitOnly);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new[] {typeof(IntPtr), typeof(Delegate)});
            var il = new GroboIL(constructor);
            il.Ldarg(0);
            il.Ldarg(1);
            il.Stfld(pointerField);
            il.Ldarg(0);
            il.Ldarg(2);
            il.Stfld(delegateField);
            il.Ret();
            var method = typeBuilder.DefineMethod("DoNothing", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), Type.EmptyTypes);
            il = new GroboIL(method);
            il.Ldarg(0);
            il.Ldfld(pointerField);
            il.Calli(CallingConventions.Standard, typeof(void), Type.EmptyTypes);
            il.Ret();
            typeBuilder.DefineMethodOverride(method, typeof(ITest).GetMethod("DoNothing"));
            typeBuilder.AddInterfaceImplementation(typeof(ITest));
            var type = typeBuilder.CreateType();
            return (ITest)Activator.CreateInstance(type, new object[] {dynamicMethodPointerExtractor((DynamicMethod)action.Item2), action.Item1});
        }

        private Tuple<Action, MethodInfo> Build()
        {
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), Type.EmptyTypes, Module, true);
            using(var il = new GroboIL(dynamicMethod))
            {
                il.Ldfld(xField);
                il.Ldc_I4(1);
                il.Add();
                il.Stfld(xField);
                il.Ret();
            }
            return new Tuple<Action, MethodInfo>((Action)dynamicMethod.CreateDelegate(typeof(Action)), dynamicMethod);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string Func7(int x)
        {
            switch (x)
            {
                case 0:
                case 2:
                    return "zzz";
                case 5:
                case 1000001:
                    return "qxx";
                case 7:
                case 1000000:
                    return "qzz";
                default:
                    return "xxx";
            }
        }

        private Func<int, string> BuildSwitch1()
        {
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(string), new[] {typeof(int)}, Module, true);
            using(var il = new GroboIL(dynamicMethod))
            {
                il.Ldarg(0);
                var zzzLabel = il.DefineLabel("zzz");
                var qxxLabel = il.DefineLabel("qxx");
                var qzzLabel = il.DefineLabel("qzz");
                var xxxLabel = il.DefineLabel("xxx");
                il.Switch(zzzLabel, xxxLabel, zzzLabel);
                il.Ldarg(0);
                il.Ldc_I4(5);
                il.Sub();
                il.Switch(qxxLabel, xxxLabel, qzzLabel);
                il.Ldarg(0);
                il.Ldc_I4(0xf4240);
                il.Sub();
                il.Switch(qzzLabel, qxxLabel);
                il.Br(xxxLabel);
                il.MarkLabel(zzzLabel);
                il.Ldstr("zzz");
                il.Ret();
                il.MarkLabel(qxxLabel);
                il.Ldstr("qxx");
                il.Ret();
                il.MarkLabel(qzzLabel);
                il.Ldstr("qzz");
                il.Ret();
                il.MarkLabel(xxxLabel);
                il.Ldstr("xxx");
                il.Ret();
            }
            return (Func<int, string>)dynamicMethod.CreateDelegate(typeof(Func<int, string>));
        }

        public static int[] testValues = new[] { 0, -1, 2, -1, -1, 5, -1, 7, 1000000, 1000001, -1, -1, -1, -1 };
        public static int[] indexes = new[] { 0, -1, 1, -1, -1, 2, -1, 3, 4, 5, -1, -1, -1, -1 };

        private Func<int, string> BuildSwitch2()
        {
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(string), new[] {typeof(int)}, Module, true);
            using(var il = new GroboIL(dynamicMethod))
            {
                var zzzLabel = il.DefineLabel("zzz");
                var qxxLabel = il.DefineLabel("qxx");
                var qzzLabel = il.DefineLabel("qzz");
                var xxxLabel = il.DefineLabel("xxx");
                var index = il.DeclareLocal(typeof(uint));
                il.Ldfld(typeof(TestPerformance).GetField("testValues"));
                il.Ldarg(0);
                il.Ldc_I4(14);
                il.Rem(true);
                il.Stloc(index);
                il.Ldloc(index);
                il.Ldelem(typeof(int));
                il.Ldarg(0);
                il.Bne_Un(xxxLabel);
                il.Ldfld(typeof(TestPerformance).GetField("indexes"));
                il.Ldloc(index);
                il.Ldelem(typeof(int));
                il.Switch(zzzLabel, zzzLabel, qxxLabel, qzzLabel, qzzLabel, qxxLabel);
                il.Br(xxxLabel);
                il.MarkLabel(zzzLabel);
                il.Ldstr("zzz");
                il.Ret();
                il.MarkLabel(qxxLabel);
                il.Ldstr("qxx");
                il.Ret();
                il.MarkLabel(qzzLabel);
                il.Ldstr("qzz");
                il.Ret();
                il.MarkLabel(xxxLabel);
                il.Ldstr("xxx");
                il.Ret();
            }
            return (Func<int, string>)dynamicMethod.CreateDelegate(typeof(Func<int, string>));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string Func8(string s)
        {
            switch (s)
            {
                case "0":
                case "2":
                    return "zzz";
                case "5":
                case "1000001":
                    return "qxx";
                case "7":
                case "1000000":
                    return "qzz";
                default:
                    return "xxx";
            }
        }

        public static string[] testValues2;
        public static int[] indexes2;

        private static void Init(string[] values)
        {
            for(int x = values.Length; ;++x)
            {
                bool[] exist = new bool[x];
                bool ok = true;
                foreach(var s in values)
                {
                    var hash = s.GetHashCode();
                    if(exist[hash % x])
                    {
                        ok = false;
                        break;
                    }
                    exist[hash % x] = true;
                }
                if(ok)
                {
                    testValues2 = new string[x];
                    indexes2 = new int[x];
                    for(int index = 0; index < values.Length; index++)
                    {
                        var s = values[index];
                        var i = s.GetHashCode() % x;
                        testValues2[i] = s;
                        indexes2[i] = index;
                    }
                    return;
                }
            }
        }

        private Func<string, string> BuildSwitch4()
        {
            Init(new[] {"0", "2", "5", "1000001", "7", "1000000"});
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(string), new[] { typeof(string) }, Module, true);
            using(var il = new GroboIL(dynamicMethod))
            {
                var zzzLabel = il.DefineLabel("zzz");
                var qxxLabel = il.DefineLabel("qxx");
                var qzzLabel = il.DefineLabel("qzz");
                var xxxLabel = il.DefineLabel("xxx");
                var index = il.DeclareLocal(typeof(uint));
                il.Ldfld(typeof(TestPerformance).GetField("testValues2"));
                il.Ldarg(0);
                il.Call(typeof(object).GetMethod("GetHashCode"), typeof(string));
                il.Ldc_I4(testValues2.Length);
                il.Rem(true);
                il.Stloc(index);
                il.Ldloc(index);
                il.Ldelem(typeof(string));
                il.Ldarg(0);
                il.Call(typeof(object).GetMethod("Equals", new[] {typeof(object)}), typeof(string));
                il.Brfalse(xxxLabel);
                il.Ldfld(typeof(TestPerformance).GetField("indexes2"));
                il.Ldloc(index);
                il.Ldelem(typeof(int));
                il.Switch(zzzLabel, zzzLabel, qxxLabel, qxxLabel, qzzLabel, qzzLabel);
                il.Br(xxxLabel);
                il.MarkLabel(zzzLabel);
                il.Ldstr("zzz");
                il.Ret();
                il.MarkLabel(qxxLabel);
                il.Ldstr("qxx");
                il.Ret();
                il.MarkLabel(qzzLabel);
                il.Ldstr("qzz");
                il.Ret();
                il.MarkLabel(xxxLabel);
                il.Ldstr("xxx");
                il.Ret();
            }
            return (Func<string, string>)dynamicMethod.CreateDelegate(typeof(Func<string, string>));
        }

        private static Func<DynamicMethod, IntPtr> EmitDynamicMethodPointerExtractor()
        {
            var method = new DynamicMethod("DynamicMethodPointerExtractor", typeof(IntPtr), new[] {typeof(DynamicMethod)}, Module, true);
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0); // stack: [dynamicMethod]
                MethodInfo getMethodDescriptorMethod = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);
                if(getMethodDescriptorMethod == null)
                    throw new MissingMethodException(typeof(DynamicMethod).Name, "GetMethodDescriptor");
                il.Call(getMethodDescriptorMethod); // stack: [dynamicMethod.GetMethodDescriptor()]
                var runtimeMethodHandle = il.DeclareLocal(typeof(RuntimeMethodHandle));
                il.Stloc(runtimeMethodHandle);
                il.Ldloc(runtimeMethodHandle);
                MethodInfo prepareMethodMethod = typeof(RuntimeHelpers).GetMethod("PrepareMethod", new[] {typeof(RuntimeMethodHandle)});
                if(prepareMethodMethod == null)
                    throw new MissingMethodException(typeof(RuntimeHelpers).Name, "PrepareMethod");
                il.Call(prepareMethodMethod);
                MethodInfo getFunctionPointerMethod = typeof(RuntimeMethodHandle).GetMethod("GetFunctionPointer", BindingFlags.Instance | BindingFlags.Public);
                if(getFunctionPointerMethod == null)
                    throw new MissingMethodException(typeof(RuntimeMethodHandle).Name, "GetFunctionPointer");
                il.Ldloca(runtimeMethodHandle);
                il.Call(getFunctionPointerMethod); // stack: [dynamicMethod.GetMethodDescriptor().GetFunctionPointer()]
                il.Ret();
            }
            return (Func<DynamicMethod, IntPtr>)method.CreateDelegate(typeof(Func<DynamicMethod, IntPtr>));
        }

        private void MeasureSpeed(ITest test, int iter)
        {
            var stopwatch = Stopwatch.StartNew();
            for(int i = 0; i < iter; ++i)
                test.DoNothing();
            var elapsed = stopwatch.Elapsed;
            Console.WriteLine(string.Format("{0:0.000} millions runs per second", iter * 1.0 / elapsed.TotalSeconds / 1000000.0));
        }

        private double MeasureSpeed<T1, T2>(Func<T1, T2> func, T1 arg, int iter, double? ethalon)
        {
            func(arg);
            var stopwatch = Stopwatch.StartNew();
            for(int i = 0; i < iter; ++i)
                func(arg);
            var elapsed = stopwatch.Elapsed;
            Console.WriteLine(string.Format("{0:0.000} millions runs per second = {1:0.000X}", iter * 1.0 / elapsed.TotalSeconds / 1000000.0, ethalon == null ? 1 : elapsed.TotalSeconds / iter / ethalon));
            return elapsed.TotalSeconds / iter;
        }

        private static readonly Func<int, int, int> func = (x, y) => x + y;
        private readonly Func<int, int, int> xfunc = (x, y) => x + y;

        private volatile bool stop;

        private static readonly FieldInfo xField = (FieldInfo)((MemberExpression)((Expression<Func<int>>)(() => x)).Body).Member;
        private static readonly Func<DynamicMethod, IntPtr> dynamicMethodPointerExtractor = EmitDynamicMethodPointerExtractor();
    }
}