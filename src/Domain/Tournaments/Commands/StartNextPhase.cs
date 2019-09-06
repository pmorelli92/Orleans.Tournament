using System;
using Orleans.Tournament.Domain.Abstractions;

namespace Orleans.Tournament.Domain.Tournaments.Commands
{
    public class StartNextPhase : ITraceable
    {
        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public StartNextPhase(Guid tournamentId, Guid traceId, Guid invokerUserId)
        {
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }
    }
}
