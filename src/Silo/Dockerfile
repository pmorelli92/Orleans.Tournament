FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14 AS build

COPY src/Silo/Silo.csproj src/Silo/Silo.csproj
COPY src/DomainC/DomainC.csproj src/DomainC/DomainC.csproj
COPY src/Projections/Projections.csproj src/Projections/Projections.csproj
COPY src/Domain.Abstractions/Domain.Abstractions.csproj src/Domain.Abstractions/Domain.Abstractions.csproj
COPY src/WebSockets/WebSockets.csproj src/WebSockets/WebSockets.csproj

RUN dotnet restore src/Silo

COPY src/Silo/ src/Silo/
COPY src/DomainC/ src/DomainC/
COPY src/Projections/ src/Projections/
COPY src/Domain.Abstractions/ src/Domain.Abstractions/
COPY src/WebSockets/ src/WebSockets/

RUN dotnet publish src/Silo -o /src/Silo/out -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.14
COPY --from=build /src/Silo/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Orleans.Tournament.Silo.dll"]