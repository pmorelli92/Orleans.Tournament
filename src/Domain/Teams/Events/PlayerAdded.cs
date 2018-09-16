using System;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Teams.Commands;

namespace Snaelro.Domain.Teams.Events
{
    public class PlayerAdded : ITraceable
    {
        public string Name { get; }

        public Guid TeamId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public PlayerAdded(string name, Guid teamId, Guid traceId, Guid invokerUserId)
        {
            Name = name;
            TeamId = teamId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

        public static PlayerAdded From(AddPlayer cmd)
            => new PlayerAdded(cmd.Name, cmd.TeamId, cmd.TraceId, cmd.InvokerUserId);
    }
}