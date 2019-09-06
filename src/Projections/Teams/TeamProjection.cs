using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Orleans.Tournament.Projections.Teams
{
    public class TeamProjection
    {
        public Guid Id { get; }

        public string Name { get; }

        public IImmutableList<string> Players { get; }

        public IImmutableList<Tournament> Tournaments { get; }

        internal TeamProjection()
        {
            Players = new List<string>().ToImmutableList();
            Tournaments = new List<Tournament>().ToImmutableList();
        }

        internal static TeamProjection New => new TeamProjection();

        [JsonConstructor]
        internal TeamProjection(
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
    }

    // Since the projection is immutable we get shortcuts for creating new instances
    internal static class TeamProjectionExtensions
    {
        internal static TeamProjection SetName(this TeamProjection @this, Guid id, string name)
            => new TeamProjection(id, name, @this.Players, @this.Tournaments);

        internal static TeamProjection AddPlayer(this TeamProjection @this, string player)
            => new TeamProjection(@this.Id, @this.Name, @this.Players.Add(player), @this.Tournaments);

        internal static TeamProjection JoinTournament(this TeamProjection @this, TeamProjection.Tournament tournamentId)
            => new TeamProjection(@this.Id, @this.Name, @this.Players, @this.Tournaments.Add(tournamentId));
    }
}
