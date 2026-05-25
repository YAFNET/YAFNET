#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 89
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ServiceStack/Directory.Build.props", "ServiceStack/"]
COPY ["YetAnotherForum.NET/YAF-MySql.csproj", "YetAnotherForum.NET/"]

COPY ["YAF.NET-MySql.slnx", ""]
COPY ["YAFNET.Core/YAFNET.Core.csproj", "YAFNET.Core/"]
COPY ["Tests/YAFNET.Tests.CoreTests/YAFNET.Tests.CoreTests.csproj", "Tests/YAFNET.Tests.CoreTests/"]
COPY ["ServiceStack/ServiceStack.OrmLite/ServiceStack.OrmLite.Core.csproj", "ServiceStack/ServiceStack.OrmLite/"]
COPY ["YAFNET.Configuration/YAFNET.Configuration.csproj", "YAFNET.Configuration/"]
COPY ["YAFNET.Types/YAFNET.Types.csproj", "YAFNET.Types/"]
COPY ["YAFNET.Data/YAFNET.Data.MySql/YAFNET.Data.MySql.csproj", "YAFNET.Data/YAFNET.Data.MySql/"]
COPY ["ServiceStack/ServiceStack.OrmLite.MySql/ServiceStack.OrmLite.MySql.Core.csproj", "ServiceStack/ServiceStack.OrmLite.MySql/"]
COPY ["WebEssentials.PWA/WebEssentials.AspNetCore.Pwa.csproj", "WebEssentials.PWA/"]
COPY ["YAFNET.Web/YAFNET.Web.csproj", "YAFNET.Web/"]
COPY . .
WORKDIR "/src/YetAnotherForum.NET"

# install NodeJS
# see https://github.com/nodesource/distributions/blob/master/README.md#deb
RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -sL https://deb.nodesource.com/setup_24.x | bash -
RUN apt-get install -y nodejs

RUN npm install

FROM build AS publish
RUN dotnet publish "YAF-MySql.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YAF.dll"]