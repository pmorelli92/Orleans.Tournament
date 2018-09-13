using System.Collections.Generic;
using System.Collections.Immutable;
using Snaelro.Domain.Teams.Events;

namespace Snaelro.Domain.Teams.ValueObjects
{
    public class State
    {
        public bool Created { get; private set; }

        public string Name { get; private set; }

        public IImmutableList<string> Players { get; private set; }

        public State()
        {
            Players = new List<string>().ToImmutableList();
        }

        public void Apply(TeamCreated @event)
        {
            Created = true;
            Name = @event.Name;
        }

        public void Apply(PlayerAdded @event)
        {
            Players = Players.Add(@event.Name);
        }
    }
}