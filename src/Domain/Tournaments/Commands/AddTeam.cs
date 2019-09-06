using System;
using Orleans.Tournament.Domain.Abstractions;

namespace Orleans.Tournament.Domain.Tournaments.Commands
{
    public class AddTeam : ITraceable
    {
        public Guid TournamentId { get; }

        public Guid TeamId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public AddTeam(Guid tournamentId, Guid teamId, Guid traceId, Guid invokerUserId)
        {
            TournamentId = tournamentId;
            TeamId = teamId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }
    }
}
