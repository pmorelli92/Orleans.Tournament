namespace Tournament.Domain.Abstractions.Events;

public record ErrorHasOccurred(
    int Code,
    string Name,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;