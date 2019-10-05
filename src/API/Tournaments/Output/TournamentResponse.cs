using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Orleans.Tournament.Domain.Tournaments;
using Orleans.Tournament.Projections.Tournaments;

namespace Orleans.Tournament.API.Tournaments.Output
{
    public class TournamentResponse
    {
        public Guid Id { get; }

        public string Name { get; }

        public IImmutableList<Team> Teams { get; }

        // I could decouple from the projection here if it is needed,
        // so if Domain changes, the client is not impacted.
        public Fixture Fixture { get; }

        public TournamentResponse(Guid id, string name, IImmutableList<Team> teams, Fixture fixture)
        {
            Id = id;
            Name = name;
            Teams = teams;
            Fixture = fixture;
        }

        public class Team
        {
            public Guid Id { get; }

            public string Name { get; }

            public Team(Guid id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        public static TournamentResponse From(TournamentProjection projection)
            => new TournamentResponse(projection.Id, projection.Name, projection.Teams.Select(e => new Team(e.Id, e.Name)).ToImmutableList(), projection.Fixture);

        public static IReadOnlyList<TournamentResponse> From(IReadOnlyList<TournamentProjection> projection)
            => projection.Select(From).ToList();
    }
}
