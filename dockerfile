FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-bionic AS base
RUN apt-get update 
RUN apt-get install -y libfreetype6 
RUN apt-get install -y libfontconfig1

WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic AS build
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
