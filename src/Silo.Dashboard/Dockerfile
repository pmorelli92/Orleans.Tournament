FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14 AS build

COPY src/Silo.Dashboard/Silo.Dashboard.csproj src/Silo.Dashboard/Silo.Dashboard.csproj

RUN dotnet restore src/Silo.Dashboard

COPY src/Silo.Dashboard/ src/Silo.Dashboard/

RUN dotnet publish src/Silo.Dashboard -o /src/Silo.Dashboard/out -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.14
COPY --from=build /src/Silo.Dashboard/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Tournament.Silo.Dashboard.dll"]