using System;
using System.Collections.Generic;
using System.Linq;
using Snaelro.Domain.Tournaments.ValueObject;
using Snaelro.Projections.Tournaments;

namespace Snaelro.API.Tournaments.Output
{
    public class TournamentResponse
    {
        public Guid Id { get; }

        public string Name { get; }

        public Fixture Fixture { get; }

        public TournamentResponse(Guid id, string name, Fixture fixture)
        {
            Id = id;
            Name = name;
            Fixture = fixture;
        }

        public static TournamentResponse From(TournamentProjection projection)
            => new TournamentResponse(projection.Id, projection.Name, projection.Fixture);

        public static IReadOnlyList<TournamentResponse> From(IReadOnlyList<TournamentProjection> projection)
            => projection.Select(From).ToList();
    }
}