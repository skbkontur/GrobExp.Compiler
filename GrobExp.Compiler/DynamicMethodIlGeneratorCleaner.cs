using System;
using System.Reflection;
using System.Reflection.Emit;

namespace GrobExp.Compiler
{
    /// <summary>
    ///     DynamicMethod keeps its DynamicILGenerator (IL, labels, fixups, tokens) alive for its whole lifetime.
    ///     After CreateDelegate the runtime uses DynamicResolver instead, so we drop the generator to free memory.
    ///     If a future runtime reads the generator after CreateDelegate, DynamicMethod will throw a NullReferenceException.
    /// </summary>
    internal static class DynamicMethodIlGeneratorCleaner
    {
        public static void Clear(DynamicMethod method)
        {
            // Mono clears the generator itself
            if (Extensions.IsMono || ilGeneratorField == null || methodHandleField == null)
                return;
            if (methodHandleField.GetValue(method) == null)
                return;
            ilGeneratorField.SetValue(method, null);
        }

        private static FieldInfo TryFindField(string name, Func<Type, bool> isExpectedType)
        {
            var field = typeof(DynamicMethod).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            return field != null && isExpectedType(field.FieldType) ? field : null;
        }

        private static bool IsIlGenerator(Type type) => typeof(ILGenerator).IsAssignableFrom(type);
        private static bool IsReferenceType(Type type) => !type.IsValueType;

        // m_* on .NET Framework and .NET 6/7
        private static readonly FieldInfo ilGeneratorField = TryFindField("_ilGenerator", IsIlGenerator) ?? TryFindField("m_ilGenerator", IsIlGenerator);
        private static readonly FieldInfo methodHandleField = TryFindField("_methodHandle", IsReferenceType) ?? TryFindField("m_methodHandle", IsReferenceType);
    }
}