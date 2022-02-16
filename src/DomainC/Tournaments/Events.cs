using Tournament.Domain.Abstractions;

namespace Tournament.Domain.Tournaments;

public record TournamentCreated(
    string Name,
    Guid TournamentId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record TeamAdded(
    Guid TeamId,
    Guid TournamentId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record TournamentStarted(
    Guid TournamentId,
    List<Guid> Teams,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record MatchResultSet(
    Guid TournamentId,
    Match Match,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;