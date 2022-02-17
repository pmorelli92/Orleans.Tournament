FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14 AS build

COPY src/API.Identity/API.Identity.csproj src/API.Identity/API.Identity.csproj

RUN dotnet restore src/API.Identity

COPY src/API.Identity/ src/API.Identity/

RUN dotnet publish src/API.Identity -o app -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.14
WORKDIR /app
COPY --from=build app .
ENTRYPOINT ["dotnet", "Tournament.API.Identity.dll"]