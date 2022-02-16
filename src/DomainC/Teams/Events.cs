using Tournament.Domain.Abstractions;

namespace Tournament.Domain.Teams;

public record TeamCreated(
    string Name,
    Guid TeamId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record PlayerAdded(
    string Name,
    Guid TeamId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record TournamentJoined(
    Guid TeamId,
    Guid TournamentId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;