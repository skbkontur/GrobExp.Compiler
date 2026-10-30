# Changelog

## v1.4.10 - 2026.10.29
- Compile lambdas into a separate collectible dynamic module per thread to avoid SIGSEGV crashes during highly concurrent compilation.
- Drop the IL generator of compiled `DynamicMethod`s after `CreateDelegate`, which kept IL, labels and fixups in memory for the method's lifetime.
- Test against net7.0, net9.0 and net10.0.
- Update GrEmit dependency to v3.6.1.

## v1.3.23 - 2021.11.30
- Add net6.0 support.
- Update GrEmit dependency to v3.4.10.

## v1.3.20 - 2021.03.11
- Update GrEmit dependency to v3.4.1.
- Run tests against net5.0 tfm.

## v1.3.10 - 2019.11.21
- Update GrEmit to v3.2.2 supporting SourceLink.

## v1.3.7 - 2019.11.21
- Use [SourceLink](https://github.com/dotnet/sourcelink) to help ReSharper decompiler show actual code.

## v1.3.2 - 2019.09.25
- Fix bug in `LambdaExpressionCreator`.
- Update dependencies.

## v1.2.1 - 2018.09.15
- Use [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning) to automate generation of assembly 
  and nuget package versions.
- Update [Gremit](https://github.com/skbkontur/gremit) dependency to v2.3.
- Fix bugs in `CallExpressionEmitter` (PR [#5](https://github.com/skbkontur/GrobExp.Compiler/pull/5)).
- Support `enum`, `decimal`, and `Nullable<TConstant>` as types of constants in expressions passed 
  to `LambdaCompiler.CompileToMethod()` (PR [#6](https://github.com/skbkontur/GrobExp.Compiler/pull/6)).

## v1.1.0 - 2018.01.07
- Support .NET Standard 2.0.
- Switch to SDK-style project format and dotnet core build tooling.
