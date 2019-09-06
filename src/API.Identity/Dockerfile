FROM mcr.microsoft.com/dotnet/core/sdk:2.1-alpine3.9 AS build

COPY src/API.Identity/API.Identity.csproj src/API.Identity/API.Identity.csproj
COPY src/Utils.Mvc/Utils.Mvc.csproj src/Utils.Mvc/Utils.Mvc.csproj

RUN dotnet restore src/API.Identity

COPY src/API.Identity/ src/API.Identity/
COPY src/Utils.Mvc/ src/Utils.Mvc/

RUN dotnet publish src/API.Identity -o out -c Release

FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.1-alpine3.9
COPY --from=build /src/API.Identity/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Orleans.Tournament.API.Identity.dll"]
EXPOSE 7004

## Local build, use from root folder of the repository
# eval $(minikube docker-env) && docker build -t ot-api-identity:local -f src/API.Identity/Dockerfile .
