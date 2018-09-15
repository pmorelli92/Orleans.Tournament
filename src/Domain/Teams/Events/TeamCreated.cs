using System;

namespace Snaelro.Domain.Teams.Events
{
    public class TeamCreated : ITraceable
    {
        public string Name { get; }

        public Guid TraceId { get; }

        public TeamCreated(string name, Guid traceId)
        {
            Name = name;
            TraceId = traceId;
        }
    }
}