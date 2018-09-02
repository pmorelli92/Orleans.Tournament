using System;

namespace Snaelro.Utils.Mvc.Responses
{
    public class ResourceResponse : TraceResponse
    {
        public Guid Id { get; }

        public ResourceResponse(Guid id, Guid traceId)
            : base(traceId)
        {
            Id = id;
        }
    }
}