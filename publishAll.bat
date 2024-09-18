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

cd ..\..

cd "Translation Editor"
dotnet publish -p:PublishProfile=linux-arm64
dotnet publish -p:PublishProfile=linux-x64
dotnet publish -p:PublishProfile=osx-arm64
dotnet publish -p:PublishProfile=osx-x64
dotnet publish -p:PublishProfile=win-x64
dotnet publish -p:PublishProfile=win-x86

cd ..

cd .\Releases

tar --strip-components 1 -acf TranslationEditor_linux-arm64.zip -C TranslationEditor\linux-arm64 .
tar --strip-components 1 -acf TranslationEditor_linux-x64.zip -C TranslationEditor\linux-x64 .
tar --strip-components 1 -acf TranslationEditor_osx-arm64.zip -C TranslationEditor\osx-arm64 .
tar --strip-components 1 -acf TranslationEditor_osx-x64.zip -C TranslationEditor\osx-x64 .
tar --strip-components 1 -acf TranslationEditor_win-x64.zip -C TranslationEditor\win-x64 .
tar --strip-components 1 -acf TranslationEditor_win-x86.zip -C TranslationEditor\win-x86 .

cd ..