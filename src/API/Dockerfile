FROM microsoft/dotnet:2.1-sdk-alpine AS build
COPY src/API/ src/API/
COPY src/Utils.Mvc/ src/Utils.Mvc/
COPY src/Domain/ src/Domain/

RUN dotnet publish src/API -o out
FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
COPY --from=build /src/API/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Snaelro.API.dll"]
EXPOSE 7000

## Local build, use from root folder of the repository
# eval $(minikube docker-env) && docker build -t snaelro-api:local -f src/API/Dockerfile .