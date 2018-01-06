dotnet build --force --no-incremental --configuration Release ./GrobExp.Compiler.sln
dotnet pack --no-build --configuration Release ./GrobExp.Compiler.sln
cd ./GrobExp.Compiler/bin/Release
dotnet nuget push *.nupkg -s https://nuget.org
pause