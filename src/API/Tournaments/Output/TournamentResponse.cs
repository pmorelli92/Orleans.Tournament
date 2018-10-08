using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Snaelro.Domain.Tournaments.ValueObject;
using Snaelro.Projections.Teams;
using Snaelro.Projections.Tournaments;

namespace Snaelro.API.Tournaments.Output
{
    public class TournamentResponse
    {
        public Guid Id { get; }

        public string Name { get; }

        public IImmutableList<Team> Teams { get; }

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