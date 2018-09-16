using System;
using Snaelro.Domain.Abstractions;

namespace Snaelro.Domain.Teams.Events
{
    public class PlayerAdded : ITraceable
    {
        public string Name { get; }

        public Guid TraceId { get; }

        public PlayerAdded(string name, Guid traceId)
        {
            Name = name;
            TraceId = traceId;
        }
    }
}