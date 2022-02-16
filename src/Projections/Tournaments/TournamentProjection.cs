using System.Collections.Immutable;
using Tournament.Domain.Tournaments;

namespace Tournament.Projections.Tournaments;

public record Team(Guid Id, string Name);

public record TournamentProjection(
    Guid Id,
    string Name,
    IImmutableList<Team> Teams,
    Fixture Fixture);