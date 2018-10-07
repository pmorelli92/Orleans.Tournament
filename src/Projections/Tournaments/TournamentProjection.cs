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

        public IImmutableList<TeamProjection> Teams { get; }

        public Fixture Fixture { get; }

        internal TournamentProjection()
        {
            Teams = new List<TeamProjection>().ToImmutableList();
        }

        internal static TournamentProjection New => new TournamentProjection();

        [JsonConstructor]
        internal TournamentProjection(
            Guid id,
            string name,
            IImmutableList<TeamProjection> teams,
            Fixture fixture)
        {
            Id = id;
            Name = name;
            Teams = teams;
            Fixture = fixture;
        }
    }

    // Since the projection is immutable we get shortcuts for creating new instances
    internal static class TournamentProjectionExtensions
    {
        internal static TournamentProjection SetName(this TournamentProjection @this, Guid id, string name)
            => new TournamentProjection(id, name, @this.Teams, @this.Fixture);

        internal static TournamentProjection AddTeam(this TournamentProjection @this, TeamProjection team)
            => new TournamentProjection(@this.Id, @this.Name, @this.Teams.Add(team), @this.Fixture);

        internal static TournamentProjection AddFixture(this TournamentProjection @this, Fixture fixture)
            => new TournamentProjection(@this.Id, @this.Name, @this.Teams, fixture);
    }
}