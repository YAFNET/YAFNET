@SET FrameworkDir=C:\Windows\Microsoft.NET\Framework\v4.0.30319
@SET FrameworkVersion=v4.0.30319
@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN

.nuget\nuget.exe restore YetAnotherForum.NET.sln
msbuild.exe YetAnotherForum.NET.sln /p:Configuration=Release /p:Platform="Any CPU" /p:WarningLevel=0;CreatePackages=true