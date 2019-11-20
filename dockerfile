FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build
WORKDIR /src
COPY api.images/api.images.csproj api.images/
RUN dotnet restore api.images/api.images.csproj
COPY . .
WORKDIR /src/api.images
RUN dotnet build api.images.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish api.images.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "api.images.dll"]
