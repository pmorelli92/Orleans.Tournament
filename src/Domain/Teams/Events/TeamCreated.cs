using System;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Teams.Commands;

namespace Orleans.Tournament.Domain.Teams.Events
{
    public class TeamCreated : ITraceable
    {
        public string Name { get; }

        public Guid TeamId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public TeamCreated(string name, Guid teamId, Guid traceId, Guid invokerUserId)
        {
            Name = name;
            TeamId = teamId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

        public static TeamCreated From(CreateTeam cmd)
            => new TeamCreated(cmd.Name, cmd.TeamId, cmd.TraceId, cmd.InvokerUserId);
    }
}
