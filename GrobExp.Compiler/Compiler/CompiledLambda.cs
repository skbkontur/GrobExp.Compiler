using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using GrobExp.Compiler.Closures;

namespace GrobExp.Compiler
{
    internal class CompiledLambda
    {
        public Delegate Delegate { get; set; }
        public MethodInfo Method { get; set; }
        public int Index { get; set; }
    }

    internal class ParsedLambda
    {
        public IClosureBuilder ClosureBuilder { get; set; }
        public Type ClosureType { get; set; }
        public ParameterExpression ClosureParameter { get; set; }
        public IClosureBuilder ConstantsBuilder { get; set; }
        public Type ConstantsType { get; set; }
        public ParameterExpression ConstantsParameter { get; set; }
        public object Constants { get; set; }
        public int DelegatesFieldId { get; set; }
        public Dictionary<ParameterExpression, int> ParsedParameters { get; set; }
        public Dictionary<ConstantExpression, int> ParsedConstants { get; set; }
        public Dictionary<SwitchExpression, Tuple<int, int, int>> ParsedSwitches { get; set; }
    }
}