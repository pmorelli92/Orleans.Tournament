using System;

namespace Snaelro.Utils.Mvc.Responses
{
    public class TraceResponse
    {
        public Guid TraceId { get; set; }

        public TraceResponse(Guid traceId)
        {
            TraceId = traceId;
        }
    }
}