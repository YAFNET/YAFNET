@SET FrameworkDir=C:\Windows\Microsoft.NET\Framework\v4.0.30319
@SET FrameworkVersion=v4.0.30319
@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN
@SET CONFIGURATION=Release

.nuget\nuget.exe restore YAF.NET.sln
"C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe" YAF.NET.sln /p:Configuration=%CONFIGURATION% /p:Platform="Any CPU" /p:WarningLevel=0 /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9