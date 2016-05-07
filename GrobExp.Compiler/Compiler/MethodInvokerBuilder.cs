using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;
using GrEmit.Utils;

namespace GrobExp.Compiler
{
    internal static class MethodInvokerBuilder
    {
        public static Delegate GetInvoker(MethodInfo method)
        {
            var result = (Delegate)hashtable[method];
            if(result == null)
            {
                lock(lockObject)
                {
                    result = (Delegate)hashtable[method];
                    if(result == null)
                        hashtable[method] = result = BuildInvoker(method);
                }
            }
            return result;
        }

        private static Delegate BuildInvoker(MethodInfo method)
        {
            var parameterTypes = new List<Type>();
            if(!method.IsStatic)
                parameterTypes.Add(method.ReflectedType);
            parameterTypes.AddRange(method.GetParameters().Select(p => p.ParameterType));
            var prefix = "MethodInvoker";
            if(method.IsStatic)
                prefix += "$" + Formatter.Format(method.DeclaringType);
            var dynamicMethod = new DynamicMethod(prefix + "$" + method.Name + "$" + Guid.NewGuid(), method.ReturnType, parameterTypes.ToArray(), typeof(MethodInvokerBuilder), true);
            using(var il = new GroboIL(dynamicMethod))
            {
                for(var i = 0; i < parameterTypes.Count; ++i)
                    il.Ldarg(i);
                il.Call(method);
                il.Ret();
            }
            return dynamicMethod.CreateDelegate(Extensions.GetDelegateType(parameterTypes.ToArray(), method.ReturnType));
        }

        private static readonly Hashtable hashtable = new Hashtable();
        private static readonly object lockObject = new object();
    }
}