using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal static class DynamicMethodInvokerBuilder
    {
        public static Type BuildDynamicMethodInvoker(ModuleBuilder module, Type[] constantTypes, Type resultType, Type[] parameterTypes)
        {
            module = module ?? LambdaCompiler.Module;
            var key = GetKey(module, constantTypes, resultType, parameterTypes);
            var type = (Type)types[key];
            if(type == null)
            {
                lock(typesLock)
                {
                    type = (Type)types[key];
                    if(type == null)
                    {
                        type = BuildDynamicMethodInvoker(module, key, constantTypes.Length, parameterTypes.Length, resultType == typeof(void));
                        types[key] = type;
                    }
                }
            }
            if(!type.IsGenericType)
                return type;
            var genericArguments = new List<Type>();
            genericArguments.AddRange(constantTypes);
            genericArguments.AddRange(parameterTypes);
            if(resultType != typeof(void))
                genericArguments.Add(resultType);
            return type.MakeGenericType(genericArguments.ToArray());
        }

        public static readonly Func<DynamicMethod, IntPtr> DynamicMethodPointerExtractor = EmitDynamicMethodPointerExtractor();

        private static Type BuildDynamicMethodInvoker(ModuleBuilder module, string name, int numberOfConstants, int numberOfParameters, bool returnsVoid)
        {
            var typeBuilder = module.DefineType(name, TypeAttributes.Public | TypeAttributes.Class);
            var names = new List<string>();
            for(var i = 0; i < numberOfConstants; ++i)
                names.Add("TConst" + (i + 1));
            for(var i = 0; i < numberOfParameters; ++i)
                names.Add("TParam" + (i + 1));
            if(!returnsVoid)
                names.Add("TResult");
            var genericParameters = typeBuilder.DefineGenericParameters(names.ToArray());
            var constantTypes = genericParameters.Take(numberOfConstants).Cast<Type>().ToArray();
            var parameterTypes = genericParameters.Skip(numberOfConstants).Take(numberOfParameters).Cast<Type>().ToArray();
            var resultType = returnsVoid ? typeof(void) : genericParameters.Last();
            var methodField = typeBuilder.DefineField("method", typeof(IntPtr), FieldAttributes.Public);
            var constantFields = new List<FieldInfo>();
            for(var i = 0; i < numberOfConstants; ++i)
                constantFields.Add(typeBuilder.DefineField("const_" + (i + 1), constantTypes[i], FieldAttributes.Public));

            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, constantTypes.Concat(new[] {typeof(IntPtr)}).ToArray());
            using(var il = new GroboIL(constructor))
            {
                for(var i = 0; i < numberOfConstants; ++i)
                {
                    il.Ldarg(0); // stack: [this]
                    il.Ldarg(i + 1); // stack: [this, arg_{i+1}]
                    il.Stfld(constantFields[i]); // this.const_{i+1} = arg_{i+1}; stack: []
                }
                il.Ldarg(0); // stack: [this]
                il.Ldarg(numberOfConstants + 1); // stack: [this, arg_{constants + 1} = method]
                il.Stfld(methodField); // this.method = method; stack: []
                il.Ret();
            }

            var method = typeBuilder.DefineMethod("Invoke", MethodAttributes.Public, resultType, parameterTypes);
            using(var il = new GroboIL(method))
            {
                for(var i = 0; i < numberOfConstants; ++i)
                {
                    il.Ldarg(0); // stack: [this]
                    il.Ldfld(constantFields[i]); // stack: [this.const_{i+1}]
                }
                for(var i = 0; i < numberOfParameters; ++i)
                    il.Ldarg(i + 1);
                il.Ldarg(0);
                il.Ldfld(methodField);
                il.Calli(CallingConventions.Standard, resultType, constantTypes.Concat(parameterTypes).ToArray());
                il.Ret();
            }

            return typeBuilder.CreateType();
        }

        private static string GetKey(ModuleBuilder module, Type[] constantTypes, Type resultType, Type[] parameterTypes)
        {
            return resultType == typeof(void)
                       ? string.Format("{0}_ActionInvoker_{1}_{2}", module.MetadataToken, constantTypes.Length, parameterTypes.Length)
                       : string.Format("{0}_FuncInvoker_{1}_{2}", module.MetadataToken, constantTypes.Length, parameterTypes.Length);
        }

        private static Func<DynamicMethod, IntPtr> EmitDynamicMethodPointerExtractor()
        {
            var method = new DynamicMethod("DynamicMethodPointerExtractor", typeof(IntPtr), new[] {typeof(DynamicMethod)}, typeof(LambdaExpressionEmitter).Module, true);
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0); // stack: [dynamicMethod]
                var getMethodDescriptorMethod = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);
                if(getMethodDescriptorMethod == null)
                    throw new MissingMethodException(typeof(DynamicMethod).Name, "GetMethodDescriptor");
                il.Call(getMethodDescriptorMethod); // stack: [dynamicMethod.GetMethodDescriptor()]
                var runtimeMethodHandle = il.DeclareLocal(typeof(RuntimeMethodHandle));
                il.Stloc(runtimeMethodHandle); // runtimeMethodHandle = dynamicMethod.GetMethodDescriptor(); stack: []
                il.Ldloc(runtimeMethodHandle); // stack: [runtimeMethodHandle]
                var prepareMethodMethod = typeof(RuntimeHelpers).GetMethod("PrepareMethod", new[] {typeof(RuntimeMethodHandle)});
                if(prepareMethodMethod == null)
                    throw new MissingMethodException(typeof(RuntimeHelpers).Name, "PrepareMethod");
                il.Call(prepareMethodMethod); // RuntimeHelpers.PrepareMethod(runtimeMethodHandle)
                var getFunctionPointerMethod = typeof(RuntimeMethodHandle).GetMethod("GetFunctionPointer", BindingFlags.Instance | BindingFlags.Public);
                if(getFunctionPointerMethod == null)
                    throw new MissingMethodException(typeof(RuntimeMethodHandle).Name, "GetFunctionPointer");
                il.Ldloca(runtimeMethodHandle); // stack: [&runtimeMethodHandle]
                il.Call(getFunctionPointerMethod); // stack: [runtimeMethodHandle.GetFunctionPointer()]
                il.Ret(); // return runtimeMethodHandle.GetFunctionPointer()
            }
            return (Func<DynamicMethod, IntPtr>)method.CreateDelegate(typeof(Func<DynamicMethod, IntPtr>));
        }

        private static readonly MethodInfo gcKeepAliveMethod = ((MethodCallExpression)((Expression<Action>)(() => GC.KeepAlive(null))).Body).Method;

        private static readonly Hashtable types = new Hashtable();
        private static readonly object typesLock = new object();
    }
}