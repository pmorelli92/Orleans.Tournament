using System;
using Orleans.Tournament.Domain.Abstractions;

namespace Orleans.Tournament.Domain.Tournaments.Commands
{
    public class CreateTournament : ITraceable
    {
        public string Name { get; }

        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public CreateTournament(string name, Guid tournamentId, Guid traceId, Guid invokerUserId)
        {
            Name = name;
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }
    }
}
