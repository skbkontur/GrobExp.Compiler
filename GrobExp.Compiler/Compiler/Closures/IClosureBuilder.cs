using System;
using System.Linq.Expressions;

namespace GrobExp.Compiler.Closures
{
    internal interface IClosureBuilder
    {
        int DefineField(Type type);
        Type Create();
        Expression MakeAccess(ParameterExpression root, int id);
        Expression Assign(ParameterExpression root, int id, Expression value);
        Expression Init(ParameterExpression root);
    }
}