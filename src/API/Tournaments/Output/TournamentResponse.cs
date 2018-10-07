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

        public IImmutableList<TeamProjection> Teams { get; }

        public Fixture Fixture { get; }

        public TournamentResponse(Guid id, string name, IImmutableList<TeamProjection> teams, Fixture fixture)
        {
            Id = id;
            Name = name;
            Teams = teams;
            Fixture = fixture;
        }

        public static TournamentResponse From(TournamentProjection projection)
            => new TournamentResponse(projection.Id, projection.Name, projection.Teams, projection.Fixture);

        public static IReadOnlyList<TournamentResponse> From(IReadOnlyList<TournamentProjection> projection)
            => projection.Select(From).ToList();
    }
}