#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Smartstore.Modules/Directory.Build.props", "src/Smartstore.Modules/"]
COPY ["Directory.Build.props", "."]
COPY ["src/Smartstore.Modules/Directory.Build.targets", "src/Smartstore.Modules/"]
COPY ["nuget.config", "."]
COPY ["src/Smartstore.Modules/Smartstore.PageBuilder/Smartstore.PageBuilder.csproj", "src/Smartstore.Modules/Smartstore.PageBuilder/"]
COPY ["src/Smartstore/Smartstore.csproj", "src/Smartstore/"]
COPY ["src/Smartstore.Core/Smartstore.Core.csproj", "src/Smartstore.Core/"]
COPY ["src/Smartstore.Web.Common/Smartstore.Web.Common.csproj", "src/Smartstore.Web.Common/"]
RUN dotnet restore "./src/Smartstore.Modules/Smartstore.PageBuilder/./Smartstore.PageBuilder.csproj"
COPY . .
WORKDIR "/src/src/Smartstore.Modules/Smartstore.PageBuilder"
RUN dotnet build "./Smartstore.PageBuilder.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Smartstore.PageBuilder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smartstore.PageBuilder.dll"]