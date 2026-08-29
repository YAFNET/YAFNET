dotnet clean YAF.NET-MySql.slnx
dotnet build YAF.NET-MySql.slnx /p:Configuration=Release /p:Platform="Any CPU" /t:Clean,Build /p:WarningLevel=0;CreatePackages=true /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9

dotnet clean YAF.NET-PostgreSQL.slnx
dotnet build YAF.NET-PostgreSQL.slnx /p:Configuration=Release /p:Platform="Any CPU" /t:Clean,Build /p:WarningLevel=0;CreatePackages=true /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9

dotnet clean YAF.NET-Sqlite.slnx
dotnet build YAF.NET-Sqlite.slnx /p:Configuration=Release /p:Platform="Any CPU" /t:Clean,Build /p:WarningLevel=0;CreatePackages=true /flp1:logfile=errors.txt;errorsonly

dotnet clean YAF.NET-SqlServer.slnx
dotnet build YAF.NET-SqlServer.slnx /p:Configuration=Release /p:Platform="Any CPU" /t:Clean,Build /p:WarningLevel=0;CreatePackages=true /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9