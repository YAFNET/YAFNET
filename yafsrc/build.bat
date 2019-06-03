@SET CONFIGURATION=Release

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -nologo -latest -property installationPath > temp.txt
set /p $MSBUILDROOT=<temp.txt
del temp.txt

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -property installationVersion > temp.txt
set /p $MSBUILDVER=<temp.txt
del temp.txt

@set $MSBUILDPATH="%$MSBUILDROOT%\MsBuild\Current\Bin\MSBuild.exe"

%$MSBUILDPATH% YAF.NET.sln /t:restore /p:Configuration=%CONFIGURATION% /p:Platform="Any CPU" /p:WarningLevel=0 /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9



