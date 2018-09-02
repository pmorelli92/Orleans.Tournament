using System;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Tournaments.Commands;

namespace Snaelro.Domain.Tournaments.Events
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