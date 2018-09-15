using System;
using System.Collections.Immutable;

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

        public static TeamResponse From(Guid id, Domain.Teams.ValueObjects.State state)
            => new TeamResponse(id, state.Name, state.Players);
    }
}