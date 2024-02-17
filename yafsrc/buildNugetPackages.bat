dotnet pack YAFNET.Types/YAFNET.Types.csproj /p:Configuration=Release 
dotnet pack YAFNET.Configuration/YAFNET.Configuration.csproj /p:Configuration=Release 
dotnet pack YAFNET.Core/YAFNET.Core.csproj /p:Configuration=Release 
dotnet pack YAFNET.Web/YAFNET.Web.csproj /p:Configuration=Release 

dotnet pack YAFNET.Data/YAFNET.Data.SqlServer/YAFNET.Data.SqlServer.csproj /p:Configuration=Release 
dotnet pack YAFNET.Data/YAFNET.Data.MySql/YAFNET.Data.MySql.csproj /p:Configuration=Release 
dotnet pack YAFNET.Data/YAFNET.Data.PostgreSQL/YAFNET.Data.PostgreSQL.csproj /p:Configuration=Release 
dotnet pack YAFNET.Data/YAFNET.Data.Sqlite/YAFNET.Data.Sqlite.csproj /p:Configuration=Release 

COPY YAFNET.Types\bin\Release\*.nupkg deploy\
COPY YAFNET.Configuration\bin\Release\*.nupkg deploy\
COPY YAFNET.Core\bin\Release\*.nupkg deploy\
COPY YAFNET.Web\bin\Release\*.nupkg deploy\

COPY YAFNET.Data\YAFNET.Data.SqlServer\bin\Release\*.nupkg deploy\
COPY YAFNET.Data\YAFNET.Data.MySql\bin\Release\*.nupkg deploy\
COPY YAFNET.Data\YAFNET.Data.PostgreSQL\bin\Release\*.nupkg deploy\
COPY YAFNET.Data\YAFNET.Data.Sqlite\bin\Release\*.nupkg deploy\