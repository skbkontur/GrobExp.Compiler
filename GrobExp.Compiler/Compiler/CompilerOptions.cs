using System;

namespace GrobExp.Compiler
{
    [Flags]
    public enum CompilerOptions
    {
        None = 0,
        CheckNullReferences = 1,
        CheckArrayIndexes = 2,
        ExtendOnAssign = 4,
        UseTernaryLogic = 8,
        CheckDictionaryKeys = 16,
        CreateDynamicClosure = 32,
        All = CheckNullReferences | CheckArrayIndexes | ExtendOnAssign | UseTernaryLogic | CheckDictionaryKeys | CreateDynamicClosure
    }
}