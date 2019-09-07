FROM mcr.microsoft.com/dotnet/core/sdk:2.1-alpine3.9 AS build

COPY src/Silo.Dashboard/Silo.Dashboard.csproj src/Silo.Dashboard/Silo.Dashboard.csproj
COPY src/Utils.Mvc/Utils.Mvc.csproj src/Utils.Mvc/Utils.Mvc.csproj

RUN dotnet restore src/Silo.Dashboard

COPY src/Silo.Dashboard/ src/Silo.Dashboard/
COPY src/Utils.Mvc/ src/Utils.Mvc/

RUN dotnet publish src/Silo.Dashboard -o out -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-alpine3.9
COPY --from=build /src/Silo.Dashboard/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Orleans.Tournament.Silo.Dashboard.dll"]
EXPOSE 7002
