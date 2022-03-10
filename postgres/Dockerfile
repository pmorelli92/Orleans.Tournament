FROM postgres:14.0-alpine
COPY /01_orleans_main.sql /docker-entrypoint-initdb.d/01.sql
COPY /02_orleans_clustering.sql /docker-entrypoint-initdb.d/02.sql
COPY /03_orleans_persistence.sql /docker-entrypoint-initdb.d/03.sql
COPY /04_read_schema.sql /docker-entrypoint-initdb.d/04.sql
COPY /05_auth_schema.sql /docker-entrypoint-initdb.d/05.sql

ENV POSTGRES_PASSWORD="dbpassword"
ENV POSTGRES_USER="dbuser"
ENV POSTGRES_DB="orleans-tournament"
