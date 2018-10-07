using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Snaelro.Projections.Teams;

namespace Snaelro.API.Teams.Output
{
    public class TeamResponse
    {
        public Guid Id { get; }

        public string Name { get; }

        public IImmutableList<string> Players { get; }

        public TeamResponse(Guid id, string name, IImmutableList<string> players)
        {
            Id = id;
            Name = name;
            Players = players;
        }

        public static TeamResponse From(TeamProjection projection)
            => new TeamResponse(projection.Id, projection.Name, projection.Players);

        public static IReadOnlyList<TeamResponse> From(IReadOnlyList<TeamProjection> projection)
            => projection.Select(From).ToList();
    }
}