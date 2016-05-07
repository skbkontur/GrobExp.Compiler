using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace GrobExp.Compiler
{
    public static class AnonymousTypeBuilder
    {
        public static Type CreateAnonymousType(Type[] types, string[] names, ModuleBuilder module)
        {
            var key = new HashtableKey(module, types, names);
            var type = (Type)anonymousTypes[key];
            if(type == null)
            {
                lock(anonymousTypesLock)
                {
                    type = (Type)anonymousTypes[key];
                    if(type == null)
                        anonymousTypes[key] = type = BuildType(types, names, module);
                }
            }
            return type;
        }

        public static bool IsDynamicAnonymousType(Type type)
        {
            return type.Name.StartsWith("<>f__DynamicAnonymousType_");
        }

        public static bool IsAnonymousType(Type type)
        {
            return type.Name.StartsWith("<>f__AnonymousType") && !type.IsArray;
        }

        private static Type BuildType(Type[] types, string[] names, ModuleBuilder module)
        {
            var length = types.Length;
            if(names.Length != length)
                throw new InvalidOperationException();
            var typeBuilder = module.DefineType("<>f__DynamicAnonymousType_" + Guid.NewGuid(), TypeAttributes.Public | TypeAttributes.Class);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, types);
            var constructorIl = constructor.GetILGenerator();
            for(var i = 0; i < length; ++i)
            {
                var field = typeBuilder.DefineField(names[i] + "_" + Guid.NewGuid(), types[i], FieldAttributes.Private | FieldAttributes.InitOnly);
                var property = typeBuilder.DefineProperty(names[i], PropertyAttributes.None, types[i], Type.EmptyTypes);
                var setter = typeBuilder.DefineMethod(names[i] + "_setter" + Guid.NewGuid(), MethodAttributes.Public, CallingConventions.HasThis, typeof(void), new[] {types[i]});
                var il = setter.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0); // stack: [this]
                il.Emit(OpCodes.Ldarg_1); // stack: [this, value]
                il.Emit(OpCodes.Stfld, field); // this.field = value
                il.Emit(OpCodes.Ret);
                property.SetSetMethod(setter);

                var getter = typeBuilder.DefineMethod(names[i] + "_getter" + Guid.NewGuid(), MethodAttributes.Public, CallingConventions.HasThis, types[i], Type.EmptyTypes);
                il = getter.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0); // stack: [this]
                il.Emit(OpCodes.Ldfld, field); // stack: [this.field]
                il.Emit(OpCodes.Ret);
                property.SetGetMethod(getter);

                constructorIl.Emit(OpCodes.Ldarg_0); // stack: [this]
                constructorIl.Emit(OpCodes.Ldarg_S, i + 1); // stack: [this, arg_i]
                constructorIl.Emit(OpCodes.Call, setter); // this.setter(arg_i)
            }
            constructorIl.Emit(OpCodes.Ret);
            return typeBuilder.CreateType();
        }

        private static readonly Hashtable anonymousTypes = new Hashtable();
        private static readonly object anonymousTypesLock = new object();

        private class HashtableKey
        {
            public HashtableKey(Module module, Type[] types, string[] names)
            {
                Module = module;
                Types = types;
                Names = names;
            }

            public override int GetHashCode()
            {
                return Module.MetadataToken * 1000000349 + Names.Aggregate(Types.Aggregate(0, (current, t) => current * 314159265 + t.GetHashCode()), (current, t) => current * 271828459 + t.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                if(!(obj is HashtableKey))
                    return false;
                if(ReferenceEquals(this, obj)) return true;
                var other = (HashtableKey)obj;
                if(Module != other.Module || Types.Length != other.Types.Length || Names.Length != other.Names.Length)
                    return false;
                if(Types.Where((t, i) => t != other.Types[i]).Any())
                    return false;
                if(Names.Where((s, i) => s != other.Names[i]).Any())
                    return false;
                return true;
            }

            public Module Module { get; set; }
            public Type[] Types { get; private set; }
            public string[] Names { get; private set; }
        }
    }
}