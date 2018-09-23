CREATE SCHEMA auth;

CREATE TABLE auth.user (
  id UUID NOT NULL,
  email TEXT NOT NULL,
  password_hash TEXT NOT NULL,
  salt_key TEXT NOT NULL,
  claims TEXT ARRAY NOT NULL,
  CONSTRAINT user_pk PRIMARY KEY (id)
);