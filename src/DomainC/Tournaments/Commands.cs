using Tournament.Domain.Abstractions;

namespace Tournament.Domain.Tournaments;

public record CreateTournament(
    string Name,
    Guid TournamentId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record AddTeam(
    Guid TournamentId,
    Guid TeamId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record StartTournament(
    Guid TournamentId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record SetMatchResult(
    Guid TournamentId,
    Match Match,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;