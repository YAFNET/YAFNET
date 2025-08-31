dotnet restore ./yafsrc/YAF.NET-SqlServer.slnx

dotnet build YAF.NET-SqlServer.slnx /p:Configuration=Release /p:Platform="Any CPU" /p:WarningLevel=0 /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9



