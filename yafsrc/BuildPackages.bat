@SET CONFIGURATION=Release

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -nologo -latest -property installationPath > temp.txt
set /p $MSBUILDROOT=<temp.txt
del temp.txt

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -property installationVersion > temp.txt
Rem lower VS version
Rem set /p $MSBUILDVER=<temp.txt
Rem higher/unique VS
for /f "delims==" %%a in (temp.txt) do set $MSBUILDVER=%%a
del temp.txt

for /f "tokens=1 delims=." %%G in ("%$MSBUILDVER%") do set Current=%%G.0
Rem VS2017 => ~\MSBuild\15.0\Bin\MSBuild.exe
Rem VS2019 => ~\MsBuild\Current\Bin\MSBuild.exe
If "%Current%" NEQ "15.0" set Current=Current

@set $MSBUILDPATH="%$MSBUILDROOT%\MsBuild\%Current%\Bin\MSBuild.exe"

dotnet restore YAF.NET-SqlServer.slnx

%$MSBUILDPATH% YAF.NET-MySql.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Clean,Build /p:WarningLevel=0;CreatePackages=true /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9
%$MSBUILDPATH% YAF.NET-PostgreSQL.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Clean,Build /p:WarningLevel=0;CreatePackages=true /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9
%$MSBUILDPATH% YAF.NET-Sqlite.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Clean,Build /p:WarningLevel=0;CreatePackages=true /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9
%$MSBUILDPATH% YAF.NET-SqlServer.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Clean,Build /p:WarningLevel=0;CreatePackages=true /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9