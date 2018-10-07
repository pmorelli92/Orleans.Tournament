using System;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Teams.Commands;

namespace Snaelro.Domain.Teams.Events
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