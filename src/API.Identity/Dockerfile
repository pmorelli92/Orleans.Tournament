FROM microsoft/dotnet:2.1-sdk-alpine AS build
COPY src/API.Identity/ src/API.Identity/
COPY src/Utils.Mvc/ src/Utils.Mvc/

RUN dotnet publish src/API.Identity -o out
FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
COPY --from=build /src/API.Identity/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Snaelro.API.Identity.dll"]
EXPOSE 7003

## Local build, use from root folder of the repository
# eval $(minikube docker-env) && docker build -t snaelro-api-identity:local -f src/API.Identity/Dockerfile .