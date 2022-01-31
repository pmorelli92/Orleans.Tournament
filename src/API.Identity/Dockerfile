FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14 AS build

COPY src/API.Identity/API.Identity.csproj src/API.Identity/API.Identity.csproj
COPY src/Utils.Mvc/Utils.Mvc.csproj src/Utils.Mvc/Utils.Mvc.csproj

RUN dotnet restore src/API.Identity

COPY src/API.Identity/ src/API.Identity/
COPY src/Utils.Mvc/ src/Utils.Mvc/

RUN dotnet publish src/API.Identity -o app -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.14
WORKDIR /app
COPY --from=build app .
ENTRYPOINT ["dotnet", "Orleans.Tournament.API.Identity.dll"]