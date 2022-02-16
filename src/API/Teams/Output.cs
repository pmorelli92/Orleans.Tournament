using System.Collections.Immutable;
using Tournament.Projections.Teams;

namespace Tournament.API.Teams;

public record Tournament(
    Guid Id,
    string Name);

public record TeamResponse(
    Guid Id,
    string Name,
    IImmutableList<string> Players,
    IImmutableList<Tournament> Tournaments)
{
    public static TeamResponse From(TeamProjection projection)
        => new(projection.Id, projection.Name, projection.Players, projection.Tournaments.Select(e => new Tournament(e.Id, e.Name)).ToImmutableList());

    public static IReadOnlyList<TeamResponse> From(IReadOnlyList<TeamProjection> projection)
        => projection.Select(From).ToList();
}