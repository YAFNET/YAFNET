@SET CONFIGURATION=Release

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -nologo -latest -property installationPath > temp.txt
set /p $MSBUILDROOT=<temp.txt
del temp.txt

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -property installationVersion > temp.txt
set /p $MSBUILDVER=<temp.txt
del temp.txt

@set $MSBUILDPATH="%$MSBUILDROOT%\MsBuild\Current\Bin\MSBuild.exe"

%$MSBUILDPATH% YAF.NET.sln /p:Configuration=Release /p:Platform="Any CPU" /p:WarningLevel=0;CreatePackages=true