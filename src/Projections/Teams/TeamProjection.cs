using System.Collections.Immutable;

namespace Tournament.Projections.Teams;

public record Tournament(Guid Id, string Name);

public record TeamProjection(
    Guid Id,
    string Name,
    IImmutableList<string> Players,
    IImmutableList<Tournament> Tournaments);