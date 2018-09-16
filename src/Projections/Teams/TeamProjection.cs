using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Snaelro.Projections.Teams
{
    public class TeamProjection
    {
        public string Name { get; }

        public IImmutableList<string> Players { get; }

        internal TeamProjection()
        {
            Players = new List<string>().ToImmutableList();
        }

        internal static TeamProjection New => new TeamProjection();

        [JsonConstructor]
        internal TeamProjection(
            string name,
            IImmutableList<string> players)
        {
            Name = name;
            Players = players;
        }
    }

    // Since the projection is immutable we get shortcuts for creating new instances
    internal static class TeamProjectionExtensions
    {
        internal static TeamProjection SetName(this TeamProjection @this, string name)
            => new TeamProjection(name, @this.Players);

        internal static TeamProjection AddPlayer(this TeamProjection @this, string player)
            => new TeamProjection(@this.Name, @this.Players.Add(player));
    }
}