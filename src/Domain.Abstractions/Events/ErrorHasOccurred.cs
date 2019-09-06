using System;

namespace Orleans.Tournament.Domain.Abstractions.Events
{
    public class ErrorHasOccurred : ITraceable
    {
        public int Code { get; }

        public string Name { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public ErrorHasOccurred(int code, string name, Guid traceId, Guid invokerUserId)
        {
            Code = code;
            Name = name;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }
    }
}
