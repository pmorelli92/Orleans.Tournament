using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json;
using Snaelro.Domain.Tournaments.ValueObject;
using Snaelro.Projections.Teams;

namespace Snaelro.Projections.Tournaments
{
    public class TournamentProjection
    {
        public Guid Id { get; }

        public string Name { get; }

        public IImmutableList<Team> Teams { get; }

        public Fixture Fixture { get; }

        internal TournamentProjection()
        {
            Teams = new List<Team>().ToImmutableList();
        }

        internal static TournamentProjection New => new TournamentProjection();

        [JsonConstructor]
        internal TournamentProjection(
            Guid id,
            string name,
            IImmutableList<Team> teams,
            Fixture fixture)
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
    }

    // Since the projection is immutable we get shortcuts for creating new instances
    internal static class TournamentProjectionExtensions
    {
        internal static TournamentProjection SetName(this TournamentProjection @this, Guid id, string name)
            => new TournamentProjection(id, name, @this.Teams, @this.Fixture);

        internal static TournamentProjection AddTeam(this TournamentProjection @this, TournamentProjection.Team team)
            => new TournamentProjection(@this.Id, @this.Name, @this.Teams.Add(team), @this.Fixture);

        internal static TournamentProjection AddFixture(this TournamentProjection @this, Fixture fixture)
            => new TournamentProjection(@this.Id, @this.Name, @this.Teams, fixture);
    }
}