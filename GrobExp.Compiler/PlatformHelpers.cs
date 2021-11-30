using System;

namespace GrobExp.Compiler
{
    public static class PlatformHelpers
    {
        static PlatformHelpers()
        {
            IsDotNet60OrGreater = HasSystemType("System.DateOnly");
        }

        public static bool IsDotNet60OrGreater { get; }

        private static bool HasSystemType(string typeName)
        {
            try
            {
                return Type.GetType(typeName) != null;
            }
            catch
            {
                return false;
            }
        }
    }
}