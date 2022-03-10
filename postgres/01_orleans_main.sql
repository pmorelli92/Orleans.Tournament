-- https://github.com/dotnet/orleans/blob/3.x/src/AdoNet/Shared/PostgreSQL-Main.sql
CREATE TABLE OrleansQuery
(
    QueryKey varchar(64) NOT NULL,
    QueryText varchar(8000) NOT NULL,

    CONSTRAINT OrleansQuery_Key PRIMARY KEY(QueryKey)
);
