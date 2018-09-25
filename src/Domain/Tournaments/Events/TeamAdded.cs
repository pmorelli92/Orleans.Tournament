using System;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Teams.Commands;
using Snaelro.Domain.Teams.Events;
using Snaelro.Domain.Tournaments.Commands;

namespace Snaelro.Domain.Tournaments.Events
{
    public class TeamAdded : ITraceable
    {
        public Guid TeamId { get; }

        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public TeamAdded(Guid teamId, Guid tournamentId, Guid traceId, Guid invokerUserId)
        {
            TeamId = teamId;
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

        public static TeamAdded From(AddTeam cmd)
            => new TeamAdded(cmd.TeamId, cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId);
    }
}