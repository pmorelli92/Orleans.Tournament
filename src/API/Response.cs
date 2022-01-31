namespace Orleans.Tournament.API;

public record TraceResponse(Guid TraceId);

public record ResourceResponse(Guid Id, Guid TraceId) : TraceResponse(TraceId);