using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageExt;
using Newtonsoft.Json;
using Orleans.Tournament.Domain.Teams.Events;

namespace Orleans.Tournament.Domain.Teams
{
    public class TeamState
    {
        public Guid Id { get; private set; }

        public bool Created { get; private set; }

        public string Name { get; private set; }

        public IImmutableList<string> Players { get; private set; }

        public IImmutableList<Guid> Tournaments { get; private set; }

        public TeamState()
        {
            Players = new List<string>().ToImmutableList();
            Tournaments = new List<Guid>().ToImmutableList();
        }

        public void Apply(TeamCreated @event)
        {
            Created = true;
            Id = @event.TeamId;
            Name = @event.Name;
        }

        public void Apply(PlayerAdded @event)
        {
            Players = Players.Add(@event.Name);
        }

        public void Apply(TournamentJoined @event)
        {
            Tournaments = Tournaments.Add(@event.TournamentId);
        }
    }
}
