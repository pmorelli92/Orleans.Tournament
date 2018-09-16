using System;
using Snaelro.Domain.Abstractions;

namespace Snaelro.Domain.Teams.Commands
{
    public class CreateTeam : ITraceable
    {
        public string Name { get; }

        public Guid TraceId { get; }

        public CreateTeam(string name, Guid traceId)
        {
            Name = name;
            TraceId = traceId;
        }
    }
}