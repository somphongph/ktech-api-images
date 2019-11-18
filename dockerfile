FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build
WORKDIR /src
COPY tripdini.images/tripdini.images.csproj tripdini.images/
RUN dotnet restore tripdini.images/tripdini.images.csproj
COPY . .
WORKDIR /src/tripdini.images
RUN dotnet build tripdini.images.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish tripdini.images.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "tripdini.images.dll"]
