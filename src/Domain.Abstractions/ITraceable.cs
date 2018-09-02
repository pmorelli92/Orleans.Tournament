using System;

namespace Snaelro.Domain.Abstractions
{
    public interface ITraceable
    {
        Guid TraceId { get; }

        Guid InvokerUserId { get; }
    }
}