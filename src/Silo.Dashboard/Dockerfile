FROM microsoft/dotnet:2.1-sdk-alpine AS build
COPY src/Silo.Dashboard/ src/Silo.Dashboard/
COPY src/Utils.Mvc/ src/Utils.Mvc/

RUN dotnet publish src/Silo.Dashboard -o out -c Release
FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
COPY --from=build /src/Silo.Dashboard/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Orleans.Tournament.Silo.Dashboard.dll"]
EXPOSE 7002

## Local build, use from root folder of the repository
# eval $(minikube docker-env) && docker build -t ot-silo-dashboard:local -f src/Silo.Dashboard/Dockerfile .
