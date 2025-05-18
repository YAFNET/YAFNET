#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 89
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ServiceStack/Directory.Build.props", "ServiceStack/"]
COPY ["YetAnotherForum.NET/YAF-SqlServer.csproj", "YetAnotherForum.NET/"]

COPY ["YAF.NET-SqlServer.slnx", ""]
COPY ["YAFNET.Core/YAFNET.Core.csproj", "YAFNET.Core/"]
COPY ["Tests/YAFNET.Tests.AdminTests/YAFNET.Tests.AdminTests.csproj", "Tests/YAFNET.Tests.AdminTests/"]
COPY ["Tests/YAFNET.Tests.CoreTests/YAFNET.Tests.CoreTests.csproj", "Tests/YAFNET.Tests.CoreTests/"]
COPY ["Tests/YAFNET.Tests.UserTests/YAFNET.Tests.UserTests.csproj", "Tests/YAFNET.Tests.UserTests/"]
COPY ["Tests/YAFNET.Tests.Utils/YAFNET.Tests.Utils.csproj", "Tests/YAFNET.Tests.Utils/"]
COPY ["ServiceStack/ServiceStack.OrmLite/ServiceStack.OrmLite.Core.csproj", "ServiceStack/ServiceStack.OrmLite/"]
COPY ["YAFNET.Configuration/YAFNET.Configuration.csproj", "YAFNET.Configuration/"]
COPY ["YAFNET.Types/YAFNET.Types.csproj", "YAFNET.Types/"]
COPY ["YAFNET.Data/YAFNET.Data.SqlServer/YAFNET.Data.SqlServer.csproj", "YAFNET.Data/YAFNET.Data.SqlServer/"]
COPY ["ServiceStack/ServiceStack.OrmLite.SqlServer/ServiceStack.OrmLite.SqlServer.Core.csproj", "ServiceStack/ServiceStack.OrmLite.SqlServer/"]
COPY ["YAFNET.Web/YAFNET.Web.csproj", "YAFNET.Web/"]
RUN dotnet restore "YAF.NET-SqlServer.slnx"
COPY . .
WORKDIR "/src/YetAnotherForum.NET"

# install NodeJS
# see https://github.com/nodesource/distributions/blob/master/README.md#deb
RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -sL https://deb.nodesource.com/setup_21.x | bash -
RUN apt-get install -y nodejs

RUN npm install


RUN dotnet build "YAF-SqlServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YAF-SqlServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YAF.dll"]