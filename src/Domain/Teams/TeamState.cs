using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageExt;
using Snaelro.Domain.Teams.Events;

namespace Snaelro.Domain.Teams
{
    public class TeamState
    {
        public Guid Id { get; private set; }

        public bool Created { get; private set; }

        public string Name { get; private set; }

        public IImmutableList<string> Players { get; private set; }

        public TeamState()
        {
            Players = new List<string>().ToImmutableList();
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
    }
}