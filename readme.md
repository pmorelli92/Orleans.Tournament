## Orleans Tournament

### Work in progress

- [ ] Update .NET Core to 3.0 and Orleans to 3.0
- [ ] Use new features of Orleans and simplify code

### Architectural Scope

- [x] Multiple silos that compose a cluster.
- [x] API that connects to silo cluster.
- [x] API with async communication for responses. (Websockets)
- [x] Stream handling for projections / web sockets using published domain events.
- [x] Business rules validations in a functional way.
- [x] Orleans dashboard for metrics.
- [x] Authentication and Authorization.
- [x] Docker images running on Alpine.
- [x] Kubernetes configuration for running the solution.
- [x] Emit business error events.
- [x] Migrate domain layer to F#
- [ ] Add event store for users to consume as fallback solution.
- [ ] Persisted reference tokens for Authentication.

### Domain Scope

- [x] Create teams.
- [x] Add players to teams.
- [x] Get a team response model by Id.
- [x] Get all teams response models.
- [x] Create a tournament for different teams.
- [x] Set match result for a tournament.
- [x] Get tournament response model by id.
- [x] Get all tournament response models.
- [x] Saga for adding tournaments to the teams participating.
- [ ] Domain tests.

### Configuration (Local)

#### API.Identity

```
<env name="BUILD_VERSION" value="0.0.1" />
<env name="ASPNETCORE_ENVIRONMENT" value="Development" />
<env name="ASPNETCORE_URLS" value="http://localhost:7004" />
<env name="JWT_ISSUER_KEY" value="mUL-M6N5]4;S9XHp" />
<env name="POSTGRES_CONNECTION" value="Server=192.168.99.100;Port=30700;User Id=dbuser;Password=dbpassword;Database=orleans-tournament" />
```

#### API

```
<env name="BUILD_VERSION" value="0.0.1" />
<env name="ASPNETCORE_ENVIRONMENT" value="Development" />
<env name="ASPNETCORE_URLS" value="http://localhost:7003" />
<env name="SERVICE_ID" value="Orleans-Tournament" />
<env name="POSTGRES_CONNECTION" value="Server=192.168.99.100;Port=30700;User Id=dbuser;Password=dbpassword;Database=orleans-tournament" />
<env name="CLUSTER_ID" value="Local" />
<env name="JWT_ISSUER_KEY" value="mUL-M6N5]4;S9XHp" />
```

#### Silo

```
<env name="GATEWAY_PORT" value="30000" />
<env name="BUILD_VERSION" value="0.0.1" />
<env name="ASPNETCORE_ENVIRONMENT" value="Development" />
<env name="ASPNETCORE_URLS" value="http://localhost:7001" />
<env name="SERVICE_ID" value="Orleans-Tournament" />
<env name="SILO_PORT" value="11111" />
<env name="POSTGRES_CONNECTION" value="Server=192.168.99.100;Port=30700;User Id=dbuser;Password=dbpassword;Database=orleans-tournament" />
<env name="CLUSTER_ID" value="Local" />
```

#### Silo Dashboard

```
<env name="ASPNETCORE_ENVIRONMENT" value="Development" />
<env name="ASPNETCORE_URLS" value="http://localhost:7002" />
<env name="SERVICE_ID" value="Orleans-Tournament" />
<env name="POSTGRES_CONNECTION" value="Server=192.168.99.100;Port=30700;User Id=dbuser;Password=dbpassword;Database=orleans-tournament" />
<env name="CLUSTER_ID" value="Local" />
<env name="BUILD_VERSION" value="0.0.1" />
```
