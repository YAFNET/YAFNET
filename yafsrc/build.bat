@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN
@SET MSBUILDPATH="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MsBuild.exe"
@SET CONFIGURATION=Release

%MSBUILDPATH% YetAnotherForum.NET.sln /t:restore /p:Configuration=%CONFIGURATION% /p:Platform="Any CPU" /p:WarningLevel=0 /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9