using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LanguageExt.UnitsOfMeasure;
using Orleans.Tournament.Projections.Teams;

namespace Orleans.Tournament.API.Teams.Output
{
    public class TeamResponse
    {
        public Guid Id { get; }

        public string Name { get; }

        public IImmutableList<string> Players { get; }

        public IImmutableList<Tournament> Tournaments { get; }

        public TeamResponse(
            Guid id,
            string name,
            IImmutableList<string> players,
            IImmutableList<Tournament> tournaments)
        {
            Id = id;
            Name = name;
            Players = players;
            Tournaments = tournaments;
        }

        public class Tournament
        {
            public Guid Id { get; }

            public string Name { get; }

            public Tournament(Guid id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        public static TeamResponse From(TeamProjection projection)
            => new TeamResponse(projection.Id, projection.Name, projection.Players, projection.Tournaments.Select(e => new Tournament(e.Id, e.Name)).ToImmutableList());

        public static IReadOnlyList<TeamResponse> From(IReadOnlyList<TeamProjection> projection)
            => projection.Select(From).ToList();
    }
}
