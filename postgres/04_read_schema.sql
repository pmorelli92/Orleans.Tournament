CREATE SCHEMA read;

CREATE TABLE read.team_projection (
  id UUID NOT NULL,
  payload JSONB NOT NULL,
  CONSTRAINT team_projection_pk PRIMARY KEY (id)
);

CREATE TABLE read.tournament_projection (
  id UUID NOT NULL,
  payload JSONB NOT NULL,
  CONSTRAINT tournament_projection_pk PRIMARY KEY (id)
);