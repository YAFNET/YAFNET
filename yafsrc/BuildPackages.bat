@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN
@SET MSBUILDPATH="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MsBuild.exe"
@SET CONFIGURATION=Release

%MSBUILDPATH%  YAF.NET.sln /t:restore /p:Configuration=Release /p:Platform="Any CPU" /p:WarningLevel=0;CreatePackages=true