using System;
using System.Reflection;

namespace GrobExp.Compiler
{
    internal class CompiledLambda
    {
        public Delegate Delegate { get; set; }
        public MethodInfo Method { get; set; }
    }
}