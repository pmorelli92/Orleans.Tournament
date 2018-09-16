using System;
using Snaelro.Domain.Abstractions;

namespace Snaelro.Domain.Teams.Commands
{
    public class AddPlayer : ITraceable
    {
        public string Name { get; }

        public Guid TraceId { get; }

        public AddPlayer(string name, Guid traceId)
        {
            Name = name;
            TraceId = traceId;
        }

    }
}