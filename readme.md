## Tournament Demo Project

This project is a backend oriented demo with websocket capabilities that relies heavily in the Actor Model Framework implementation of Microsoft Orleans. I gave myself the luxury of experimenting with some technologies and practices that might be useful on a real world distributed system.

| **Table of Contents**  |
|---|
| [Demo](#demo)  |
|  [Domain](#domain) |
| [DDD and immutability](#ddd-and-immutability)  |
| [CQRS, projections, event sourcing and eventual consistency](#cqrs-projections-event-sourcing-and-eventual-consistency) |
| [Sagas](#sagas) |
| [Async communication via websockets](#async-communication-via-websockets) |
| [Authentication](#authentication) |
| [Infrastructure (Kubernetes)](#infrastructure-kubernetes) |
| [How to run it](#how-to-run-it) |
| [Tests](#tests) |

## Demo

Click to view it on Loom with sound.

[![demo](https://cdn.loom.com/sessions/thumbnails/295a8f4dd71a474cb59b09388422deb9-with-play.gif)](https://www.loom.com/share/295a8f4dd71a474cb59b09388422deb9)

## Domain

The domain contains two aggregate roots: `Teams` and `Tournaments`.

A `Team` contains a name, a list of players (strings) and a list of tournaments IDs on which the team is registered.

A `Tournament` contains a name, a list of teams IDs participating on it, and a `Fixture` that contains information of each different `Phase`: quarter finals, semi finals and finals.

For a Tournament to start, the list of teams must have a count of 8. When a tournament starts, the participating teams are shuffled randomly, and the `Fixture`'s quarter `Phase` is populated.

A `Phase` is a container for a list of `Match`es that belong to the bracket. Each `Match` contains the `LocalTeamId`, the `AwayTeamId` and a `MatchResult`.

In order not to use null values, the `MatchResult` is created with default values of 0 goals for each team and the property `Played` set to false.

When the user intents the command `SetMatchResult` the correct result will be assigned to the corresponding `Match` on the corresponding `Phase`.

When all the `Match`es on a `Phase` are played, the next phase is generated. Meanwhile the quarter finals are generated by a random shuffle of the participating teams, the semifinals and the finals are not generated randomly but considering the results of previous brackets.

## DDD and immutability

In Actor Model Framework, each Actor (instance of an aggregate root) contains a state that mutates when applying events. Orleans is not different, and each Grain (Actor) acts the same way.

1. Each `TournamentGrain` contains a `TournamentState`.
2. When a `TournamentGrain` receives a `Command`, it first checks if the command is valid business wise.
3. If the `Command` is valid, it will publish an `Event` informing what happened and modifying the `State` accordingly.

The `TournamentState` is not coupled to Orleans framework. This means that the class is just a plain one that exposes methods that acts on a certain event. This allows for replayability of events and event sourcing as a whole.

The state is then mutable, but that does not mean that the properties exposed by it are mutable as well. For example the `TournamentState` contains the `Fixture`.

The `Fixture` is a value record implemented with C# records. This means that the methods exposed by it does not cause side effects and instead returns a new `Fixture` with the changes reflected. This enables an easier testability and predictability as all the methods are deterministic; you can execute a method a hundred times and the result is the same.

## CQRS, projections, event sourcing and eventual consistency

One of the advantages of using an Actor Model Framework is the innate segregation of commands and queries. The commands are executed against a certain Actor (Grain) and causes side effects, such as publishing an Event. However, the queries do not go against a Grain, instead they go against a read database that gets populated via projections.

![CQRS and projections.](/img/projections.drawio.png)

This is, in fact, eventual consistency. There are some milliseconds between the source of truth (Grain) changes to be reflected in the projections that are queryable. This should not be a problem, but one needs to make sure to enable retry strategies in case the database is down while consuming an event from the stream, etc.

Now, imagine that the system goes down, and you want to invoke the `SetMatchResult` command:

1. `TournamentGrain` is not on memory.
2. Initialize the `TournamentGrain` by replaying the Events stored in the write database.
3. Executes the `SetMatchResult` command, etc.

As you can see, the Grain will never use the read state as the source of truth, and instead it will rely on the event sourcing mechanism, to apply all the events again one after another until the state is up to date.

## Sagas

`Saga` is a pattern for a distributed transaction that impacts more than one aggregate. In this case, lets look at the scenario when a `Team` wants to join a `Tournament`.

1. `TeamGrain` receives the command `CreateTeam` and the `TeamState` is initialized.
2. `TournamentGrain` receives the command `AddTeam` for the team created above, validations kick in:
	- Does the team exist? (Note: here you are not supposed to query your read database, as it not the source of truth, but actually send a message to the `TeamGrain`).
	- Does the tournament contain less than 8 teams?
	- Is the tournament already started?
3. Publishes the `TeamAdded` event.

So far, the `TournamentGrain` is aware of the Team joining, but the `TeamGrain` is not aware of the participation of a Tournament. Enter the `TeamAddedSaga` Grain.

1. `TeamAddedSaga` subscribes implicitly to an Orleans Stream looking for `TeamAdded` events.
2. When receives the event, it gets a reference for the `TeamGrain` with the corresponding ID.
3. Uses the `TeamGrain` to publish the `JoinTournament` command.
4. `TeamGrain` receives the command `JoinTournament`.
5. As this is already validated, skip validations. This is because the `TournamentGrain` already did the required validations.
6. Publishes the `TeamJoinedTournament` event.

In this case there is not a rollback strategy but these capabilities can definitely be handled with an "intermediate" Grain such as the Saga.

## Async communication via websockets

Given the async nature of Orleans, the commands executed by a user should not return results on how does the resource look after a command succeeded, not even IF the command succeeded.

Instead the response will contain a `TraceId` as a GUID representation of the intent of the user. The user will receive a message via websockets indicating what happened for that TraceId. The frontend can reflect the changes accordingly.

![Websockets](/img/websockets.drawio.png)

For the graph above the work, the user should establish a websocket connection with the API. The user will only get the results for those commands he/she invoked and not for everything happening in the system.

## Authentication

As not all users should receive the results for all the commands, there is the need to distinguish users. For this purpose a super simple authentication and authorization mechanism is implemented. If you need to implement auth in a real world app, please don't reinvent the wheel and check existing solutions such as AD or Okta.

As mentioned before, each command executed will result on a `TraceId`, but internally there is also an `InvokerUserId` propagated that can also serve as an audit log.

The user will only get websocket messages for those events on which the `InvokerUserId` matches with the one on the JWT Token.

**Fallback strategies**

It makes sense to have a fallback mechanism in case a websocket event did not arrive due to network issues. This could be a key value database where a user can search for a `TraceId` to find the response to react for a command executed. This is not currently implemented but will be added later.

## Infrastructure (Kubernetes)

The solution consists on different projects that need to be executed at the same time:

- API.Identity: Create user and generate token.
- API: Entrypoint for the user to invoke commands, connects to the Orleans cluster as a client.
- Silo: Orleans cluster, handles everything related to Orleans such as placement, streams, etc.
- Silo.Dashboard: Orleans client that displays metrics and information.

It also requires an instance of Postgres to be running and also accessible for connection.

Taking in consideration that the API could be split in smaller ones and the Silos should be scalable, I decided to create Kubernetes manifests to host this application. As it is not a common practice to have your own Kubernetes cluster locally, I also decided on using `Kind` which uses the docker engine to host a single node of Kubernetes locally.

I am not going to enter into details of how this works, but just so you know that `Docker Desktop` and `Kind` are required to be installed to run this solution. Of course, one can choose to run locally instead, bear in mind that in that scenario you need to be able to connect to a Postgres instance as mentioned above.

All the Kubernetes configuration files can be found on the `kubernetes` folder. **NOTE:** *There are also "secrets" on the mentioned folder, for a real world application please do not store your secrets in plain text on your repository*.

## How to run it

As mentioned before, it is required to have docker installed as well as `kind` command line and `kubectl`. It is also suggested to have `make` support, so you don't have to manually go through the commands one by one.

Initial bootstrap:

```
make run
```

Trigger rebuild of images and recreation of all the pods:

```
make restart
```

If you see something like this, everything should be up and running:

![running terminal](/img/running.png)

Now you can access with the following endpoints:

- [API (Port 30703)](http://localhost:30703/version)
- [API Identity (Port 30704)](http://localhost:30704/version)
- [Silo Dashboard (Port 30702)](http://localhost:30702)

You can use the Postman collection below to try out the functionality as you can see on the demo. This collection assigns values from previous responses so you don't have to modify the requests manually but just execute them in order.

[![Run in Postman](https://run.pstmn.io/button.svg)](https://god.gw.postman.com/run-collection/7368453-13049026-2905-4a52-979d-4e619e8c806c?action=collection%2Ffork&collection-url=entityId%3D7368453-13049026-2905-4a52-979d-4e619e8c806c%26entityType%3Dcollection%26workspaceId%3De2e94935-58da-4b5e-9215-d61b78393014#?env%5BOrleans%20Tournament%5D=W3sia2V5IjoiQXBpSWRlbnRpdHlVcmwiLCJ2YWx1ZSI6Imh0dHA6Ly9sb2NhbGhvc3Q6MzA3MDQiLCJlbmFibGVkIjp0cnVlLCJzZXNzaW9uVmFsdWUiOiJodHRwOi8vbG9jYWxob3N0OjMwNzA0Iiwic2Vzc2lvbkluZGV4IjowfSx7ImtleSI6IkFwaVVybCIsInZhbHVlIjoiaHR0cDovL2xvY2FsaG9zdDozMDcwMyIsImVuYWJsZWQiOnRydWUsInNlc3Npb25WYWx1ZSI6Imh0dHA6Ly9sb2NhbGhvc3Q6MzA3MDMiLCJzZXNzaW9uSW5kZXgiOjF9LHsia2V5IjoiU2lsb0Rhc2hib2FyZFVybCIsInZhbHVlIjoiaHR0cDovL2xvY2FsaG9zdDozMDcwMiIsImVuYWJsZWQiOnRydWUsInNlc3Npb25WYWx1ZSI6Imh0dHA6Ly9sb2NhbGhvc3Q6MzA3MDIiLCJzZXNzaW9uSW5kZXgiOjJ9LHsia2V5IjoiU2lsb1VybCIsInZhbHVlIjoiaHR0cDovL2xvY2FsaG9zdDozMDcwMSIsImVuYWJsZWQiOnRydWUsInNlc3Npb25WYWx1ZSI6Imh0dHA6Ly9sb2NhbGhvc3Q6MzA3MDEiLCJzZXNzaW9uSW5kZXgiOjN9XQ==)

## Tests

Tests are not yet implemented, but you can see a simple example on the `Domain.Tests` project. I would like the unit tests just to cover the domain functionality:

- Given a state, apply an event, assert the modified state.
- Given a value object, invoke a method, assert the returned record.
