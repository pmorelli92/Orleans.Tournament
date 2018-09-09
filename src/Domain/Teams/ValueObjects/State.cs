using System.Collections.Generic;
using System.Collections.Immutable;
using Snaelro.Domain.Teams.Events;

namespace Snaelro.Domain.Teams.ValueObjects
{
    public class State
    {
        public IImmutableList<string> Messages { get; private set; }

        public State()
        {
            Messages = new List<string>().ToImmutableList();
        }

        public void Apply(Echoed @event)
        {
            Messages = Messages.Add(@event.Message);
        }
    }
}