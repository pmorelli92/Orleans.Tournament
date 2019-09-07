FROM mcr.microsoft.com/dotnet/core/sdk:2.1-alpine3.9 AS build

COPY src/API.Identity/API.Identity.csproj src/API.Identity/API.Identity.csproj
COPY src/Utils.Mvc/Utils.Mvc.csproj src/Utils.Mvc/Utils.Mvc.csproj

RUN dotnet restore src/API.Identity

COPY src/API.Identity/ src/API.Identity/
COPY src/Utils.Mvc/ src/Utils.Mvc/

RUN dotnet publish src/API.Identity -o out -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-alpine3.9
COPY --from=build /src/API.Identity/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Orleans.Tournament.API.Identity.dll"]
EXPOSE 7004
