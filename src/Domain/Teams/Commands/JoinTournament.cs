using System;
using Snaelro.Domain.Abstractions;

namespace Snaelro.Domain.Teams.Commands
{
    public class JoinTournament : ITraceable
    {
        public Guid TeamId { get; }

        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public JoinTournament(Guid teamId, Guid tournamentId, Guid traceId, Guid invokerUserId)
        {
            TeamId = teamId;
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

    }
}