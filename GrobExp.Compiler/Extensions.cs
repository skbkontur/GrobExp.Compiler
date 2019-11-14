using System;
using System.Collections.Generic;
using System.Linq;

namespace GrobExp.Compiler
{
    public static class Extensions
    {
        static Extensions()
        {
            var systemTypes = typeof(Func<>).Assembly.ExportedTypes.ToArray();

            funcTypesByNumberOfGenericParameters = systemTypes.Where(type => type.FullName?.StartsWith("System.Func`") == true)
                                                              .ToDictionary(type => type.GetGenericArguments().Length);

            actionTypesByNumberOfGenericParameters = systemTypes.Where(type => type.FullName?.StartsWith("System.Action`") == true)
                                                                .ToDictionary(type => type.GetGenericArguments().Length);
        }

        public static bool IsMono { get; } = Type.GetType("Mono.Runtime") != null;

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        public static bool IsList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsDictionary(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public static bool IsStaticClosure(this Type type)
        {
            return type.IsNested && type.DeclaringType == typeof(StaticClosures);
        }

        public static bool Unsigned(this Type type)
        {
            if (type == typeof(IntPtr))
                return false;
            if (type == typeof(UIntPtr))
                return true;
            switch (Type.GetTypeCode(type))
            {
            case TypeCode.Boolean:
            case TypeCode.Byte:
            case TypeCode.Char:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Single:
            case TypeCode.Double:
                return false;
            default:
                throw new InvalidOperationException($"Type '{type}' cannot be used in comparison operations");
            }
        }

        public static Type GetDelegateType(Type[] parameterTypes, Type returnType)
        {
            if (returnType == typeof(void))
            {
                if (parameterTypes.Length == 0)
                    return typeof(Action);

                if (actionTypesByNumberOfGenericParameters.TryGetValue(parameterTypes.Length, out var actionType))
                    return actionType.MakeGenericType(parameterTypes);

                throw new NotSupportedException($"Too many parameters for creating Action type: {parameterTypes.Length}");
            }

            parameterTypes = parameterTypes.Concat(new[] {returnType}).ToArray();

            if (funcTypesByNumberOfGenericParameters.TryGetValue(parameterTypes.Length, out var funcType))
                return funcType.MakeGenericType(parameterTypes);

            throw new NotSupportedException($"Too many parameters for creating Func type: {parameterTypes.Length}");
        }

        private static readonly Dictionary<int, Type> funcTypesByNumberOfGenericParameters;
        private static readonly Dictionary<int, Type> actionTypesByNumberOfGenericParameters;
    }
}