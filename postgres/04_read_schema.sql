CREATE SCHEMA read;

CREATE TABLE read.team_projection (
  id UUID NOT NULL,
  payload JSONB NOT NULL,
  CONSTRAINT projection_pk PRIMARY KEY (id)
);