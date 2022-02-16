using Tournament.Domain.Abstractions;

namespace Tournament.Domain.Teams;

public record CreateTeam(
    string Name,
    Guid TeamId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record AddPlayer(
    string Name,
    Guid TeamId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;

public record JoinTournament(
    Guid TeamId,
    Guid TournamentId,
    Guid TraceId,
    Guid InvokerUserId) : ITraceable;