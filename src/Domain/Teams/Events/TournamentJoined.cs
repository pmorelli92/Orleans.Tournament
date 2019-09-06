using System;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Teams.Commands;

namespace Orleans.Tournament.Domain.Teams.Events
{
    public class TournamentJoined : ITraceable
    {
        public Guid TeamId { get; }

        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public TournamentJoined(Guid teamId, Guid tournamentId, Guid traceId, Guid invokerUserId)
        {
            TeamId = teamId;
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

        public static TournamentJoined From(JoinTournament cmd)
            => new TournamentJoined(cmd.TeamId, cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId);
    }
}
