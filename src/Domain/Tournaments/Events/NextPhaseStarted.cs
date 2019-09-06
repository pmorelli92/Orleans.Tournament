using System;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Tournaments.Commands;

namespace Orleans.Tournament.Domain.Tournaments.Events
{
    public class NextPhaseStarted : ITraceable
    {
        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public NextPhaseStarted(Guid tournamentId, Guid traceId, Guid invokerUserId)
        {
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

        public static NextPhaseStarted From(StartNextPhase cmd)
            => new NextPhaseStarted(cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId);
    }
}
