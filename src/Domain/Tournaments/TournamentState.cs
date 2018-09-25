using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Snaelro.Domain.Tournaments.Events;

namespace Snaelro.Domain.Tournaments
{
    public class TournamentState
    {
        public Guid Id { get; private set; }

        public bool Created { get; private set; }

        public string Name { get; private set; }

        public IImmutableList<Guid> Teams { get; private set; }

        public TournamentState()
        {
            Teams = new List<Guid>().ToImmutableList();
        }

        public void Apply(TournamentCreated @event)
        {
            Created = true;
            Id = @event.TournamentId;
            Name = @event.Name;
        }

        public void Apply(TeamAdded @event)
        {
            Teams = Teams.Add(@event.TeamId);
        }
    }
}