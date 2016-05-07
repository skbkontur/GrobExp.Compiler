using System.Runtime.CompilerServices;

namespace GrobExp.Compiler.ExpressionEmitters
{
    public class RuntimeVariables : IRuntimeVariables
    {
        public RuntimeVariables(object[] values)
        {
            this.values = values;
        }

        public int Count { get { return values.Length; } }

        public object this[int index] { get { return values[index]; } set { values[index] = value; } }
        private readonly object[] values;
    }
}