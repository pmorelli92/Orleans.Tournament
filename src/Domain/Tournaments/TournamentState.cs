using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using Newtonsoft.Json;
using Snaelro.Domain.Tournaments.Events;
using Snaelro.Domain.Tournaments.ValueObject;

namespace Snaelro.Domain.Tournaments
{
    public class TournamentState
    {
        public Guid Id { get; private set; }

        public bool Created { get; private set; }

        public string Name { get; private set; }

        public IImmutableList<Guid> Teams { get; private set; }

        public Fixture Fixture { get; private set; }

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

        public void Apply(TournamentStarted @event)
        {
            Fixture = Fixture.Create(@event.Teams);
        }

        public void Apply(MatchResultSet @event)
        {
            Fixture.SetMatchResult(@event.MatchResult);
        }

        public void Apply(NextPhaseStarted @event)
        {
            Fixture.StartNextPhase();
        }
    }
}