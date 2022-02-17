FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14 AS build

COPY src/API/API.csproj src/API/API.csproj
COPY src/Domain/Domain.csproj src/Domain/Domain.csproj
COPY src/Projections/Projections.csproj src/Projections/Projections.csproj
COPY src/Domain.Abstractions/Domain.Abstractions.csproj src/Domain.Abstractions/Domain.Abstractions.csproj
COPY src/WebSockets/WebSockets.csproj src/WebSockets/WebSockets.csproj

RUN dotnet restore src/API

COPY src/API/ src/API/
COPY src/Domain/ src/Domain/
COPY src/Projections/ src/Projections/
COPY src/Domain.Abstractions/ src/Domain.Abstractions/
COPY src/WebSockets/ src/WebSockets/

RUN dotnet publish src/API -o src/API/out -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.14
COPY --from=build /src/API/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Tournament.API.dll"]