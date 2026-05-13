#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SalesProject.WebApi/SalesProject.WebApi.csproj", "src/SalesProject.WebApi/"]
COPY ["src/SalesProject.IoC/SalesProject.IoC.csproj", "src/SalesProject.IoC/"]
COPY ["src/SalesProject.Domain/SalesProject.Domain.csproj", "src/SalesProject.Domain/"]
COPY ["src/SalesProject.Common/SalesProject.Common.csproj", "src/SalesProject.Common/"]
COPY ["src/SalesProject.Application/SalesProject.Application.csproj", "src/SalesProject.Application/"]
COPY ["src/SalesProject.ORM/SalesProject.ORM.csproj", "src/SalesProject.ORM/"]
RUN dotnet restore "./src/SalesProject.WebApi/SalesProject.WebApi.csproj"
COPY . .
WORKDIR "/src/src/SalesProject.WebApi"
RUN dotnet build "./SalesProject.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SalesProject.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SalesProject.WebApi.dll"]