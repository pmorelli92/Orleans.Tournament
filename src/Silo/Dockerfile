FROM microsoft/dotnet:2.1-sdk-alpine AS build
COPY src/Silo/ src/Silo/
COPY src/Utils.Mvc/ src/Utils.Mvc/
COPY src/Domain/ src/Domain/
COPY src/Domain.Abstractions/ src/Domain.Abstractions/
COPY src/Projections/ src/Projections/
COPY src/WebSockets/ src/WebSockets/

RUN dotnet publish src/Silo -o out
FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
COPY --from=build /src/Silo/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Snaelro.Silo.dll"]
EXPOSE 7001

## Local build, use from root folder of the repository
# eval $(minikube docker-env) && docker build -t snaelro-silo:local -f src/Silo/Dockerfile .