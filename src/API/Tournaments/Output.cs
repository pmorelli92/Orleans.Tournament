using System.Collections.Immutable;
using Tournament.Domain.Tournaments;
using Tournament.Projections.Tournaments;

namespace Tournament.API.Tournaments;

public record Team(
    Guid Id,
    string Name);

public record TournamentResponse(
    Guid Id,
    string Name,
    IImmutableList<Team> Teams,
    Fixture Fixture)
{
    public static TournamentResponse From(TournamentProjection projection)
        => new(projection.Id, projection.Name, projection.Teams.Select(e => new Team(e.Id, e.Name)).ToImmutableList(), projection.Fixture);
    public static IReadOnlyList<TournamentResponse> From(IReadOnlyList<TournamentProjection> projection)
        => projection.Select(From).ToList();
}