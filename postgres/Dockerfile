FROM postgres:11-alpine
COPY /01_orleans_clustering_table.sql /docker-entrypoint-initdb.d/01.sql
COPY /02_orleans_clustering_data.sql /docker-entrypoint-initdb.d/02.sql
COPY /03_orleans_persistence.sql /docker-entrypoint-initdb.d/03.sql

ENV POSTGRES_PASSWORD="dbpassword"
ENV POSTGRES_USER="dbuser"
ENV POSTGRES_DB="snaelro"

## Local build, use from root folder of the repository
# eval $(minikube docker-env) && docker build -t snaelro-postgres:local -f Dockerfile .