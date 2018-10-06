using System;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Tournaments.Commands;

namespace Snaelro.Domain.Tournaments.Events
{
    public class TournamentStarted : ITraceable
    {
        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public TournamentStarted(Guid tournamentId, Guid traceId, Guid invokerUserId)
        {
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

        public static TournamentStarted From(StartTournament cmd)
            => new TournamentStarted(cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId);
    }
}