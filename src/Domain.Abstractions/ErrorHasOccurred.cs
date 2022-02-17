namespace Tournament.Domain.Abstractions;

public record ErrorHasOccurred(
    int Code,
    string Name,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;