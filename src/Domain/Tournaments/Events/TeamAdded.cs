using System;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Teams.Commands;
using Orleans.Tournament.Domain.Teams.Events;
using Orleans.Tournament.Domain.Tournaments.Commands;

namespace Orleans.Tournament.Domain.Tournaments.Events
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
