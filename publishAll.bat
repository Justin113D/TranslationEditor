echo off
cd /D "%~dp0"

if exist ".\Releases" (
	rmdir ".\Releases" /s /q
)

mkdir .\Releases
cd .\Releases

mkdir .\TranslationEditor
cd .\TranslationEditor

mkdir .\linux-arm64
mkdir .\linux-x64
mkdir .\osx-arm64
mkdir .\osx-x64
mkdir .\win-x64
mkdir .\win-x86
mkdir .\bin

cd ..

mkdir .\TranslationFormatEditor
cd .\TranslationFormatEditor

mkdir .\linux-arm64
mkdir .\linux-x64
mkdir .\osx-arm64
mkdir .\osx-x64
mkdir .\win-x64
mkdir .\win-x86
mkdir .\bin

cd ..\..

cd "Translation Editor"
dotnet publish -p:PublishProfile=linux-arm64
dotnet publish -p:PublishProfile=linux-x64
dotnet publish -p:PublishProfile=osx-arm64
dotnet publish -p:PublishProfile=osx-x64
dotnet publish -p:PublishProfile=win-x64
dotnet publish -p:PublishProfile=win-x86
dotnet publish -p:PublishProfile=bin

cd ..

cd "Translation Format Editor"
dotnet publish -p:PublishProfile=linux-arm64
dotnet publish -p:PublishProfile=linux-x64
dotnet publish -p:PublishProfile=osx-arm64
dotnet publish -p:PublishProfile=osx-x64
dotnet publish -p:PublishProfile=win-x64
dotnet publish -p:PublishProfile=win-x86
dotnet publish -p:PublishProfile=bin

cd ..

cd .\Releases

set te_version="1.0.2"
set tfe_version="1.0.2"

tar --strip-components 1 -acf TranslationEditor-%te_version%-linux-arm64.zip -C TranslationEditor\linux-arm64 .
tar --strip-components 1 -acf TranslationEditor-%te_version%-linux-x64.zip -C TranslationEditor\linux-x64 .
tar --strip-components 1 -acf TranslationEditor-%te_version%-osx-arm64.zip -C TranslationEditor\osx-arm64 .
tar --strip-components 1 -acf TranslationEditor-%te_version%-osx-x64.zip -C TranslationEditor\osx-x64 .
tar --strip-components 1 -acf TranslationEditor-%te_version%-win-x64.zip -C TranslationEditor\win-x64 .
tar --strip-components 1 -acf TranslationEditor-%te_version%-win-x86.zip -C TranslationEditor\win-x86 .
tar --strip-components 1 -acf TranslationEditor-%te_version%-bin.zip -C TranslationEditor\bin .

tar --strip-components 1 -acf TranslationFormatEditor-%tfe_version%-linux-arm64.zip -C TranslationFormatEditor\linux-arm64 .
tar --strip-components 1 -acf TranslationFormatEditor-%tfe_version%-linux-x64.zip -C TranslationFormatEditor\linux-x64 .
tar --strip-components 1 -acf TranslationFormatEditor-%tfe_version%-osx-arm64.zip -C TranslationFormatEditor\osx-arm64 .
tar --strip-components 1 -acf TranslationFormatEditor-%tfe_version%-osx-x64.zip -C TranslationFormatEditor\osx-x64 .
tar --strip-components 1 -acf TranslationFormatEditor-%tfe_version%-win-x64.zip -C TranslationFormatEditor\win-x64 .
tar --strip-components 1 -acf TranslationFormatEditor-%tfe_version%-win-x86.zip -C TranslationFormatEditor\win-x86 .
tar --strip-components 1 -acf TranslationFormatEditor-%tfe_version%-bin.zip -C TranslationFormatEditor\bin .

cd ..