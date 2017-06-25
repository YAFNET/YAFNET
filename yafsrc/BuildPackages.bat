@SET FrameworkDir=C:\Windows\Microsoft.NET\Framework\v4.0.30319
@SET FrameworkVersion=v4.0.30319
@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN

.nuget\nuget.exe restore YetAnotherForum.NET.sln
"C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe" YetAnotherForum.NET.sln /p:Configuration=Release /p:Platform="Any CPU" /p:WarningLevel=0;CreatePackages=true