@SET CONFIGURATION=Release

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -nologo -latest -property installationPath > temp.txt
set /p $MSBUILDROOT=<temp.txt
del temp.txt

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -property installationVersion > temp.txt
set /p $MSBUILDVER=<temp.txt
del temp.txt

for /f "tokens=1 delims=." %%G in ("%$MSBUILDVER%") do set Current=%%G.0
Rem VS2017 => ~\MSBuild\15.0\Bin\MSBuild.exe
Rem VS2019 => ~\MsBuild\Current\Bin\MSBuild.exe
If "%Current%" NEQ "15.0" set Current=Current

@set $MSBUILDPATH="%$MSBUILDROOT%\MsBuild\%Current%\Bin\MSBuild.exe"

%$MSBUILDPATH% YAF.NET.sln /p:Configuration=%CONFIGURATION% /p:Platform="Any CPU" /p:WarningLevel=0 /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9



