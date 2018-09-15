using System;

namespace Snaelro.Domain
{
    public interface ITraceable
    {
        Guid TraceId { get; }
    }
}