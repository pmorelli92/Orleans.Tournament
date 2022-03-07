namespace Tournament.Domain.Tournaments;

public partial class TournamentState
{
    public Results TournamentExists()
        => Created
            ? Results.Unit
            : Results.TournamentDoesNotExist;

    public Results TournamentDoesNotExists()
        => !Created
            ? Results.Unit
            : Results.TournamentAlreadyExist;

    public Results TeamIsNotAdded(Guid teamId)
        => !Teams.Contains(teamId)
            ? Results.Unit
            : Results.TeamIsAlreadyAdded;

    public Results LessThanEightTeams()
        => Teams.Count < 8
            ? Results.Unit
            : Results.TournamentHasMoreThanEightTeams;

    public Results EightTeamsToStartTournament()
        => Teams.Count == 8
            ? Results.Unit
            : Results.TournamentCantStartWithLessThanEightTeams;

    public Results TournamentStarted()
        => Fixture.Quarter.IsEmpty == false
            ? Results.Unit
            : Results.TournamentDidNotStart;

    public Results TournamentDidNotStart()
        => Fixture.Quarter.IsEmpty
            ? Results.Unit
            : Results.TournamentAlreadyStarted;

    public Results MatchExistsAndIsNotPlayed(Match match)
    {
        if (Fixture.Quarter.IsEmpty)
            return Results.TournamentDidNotStart;

        // Get current phase
        var currentPhase = Fixture.Quarter;

        if (!Fixture.Final.IsEmpty)
            currentPhase = Fixture.Final;
        else if (!Fixture.Semi.IsEmpty)
            currentPhase = Fixture.Semi;

        var currentMatch = currentPhase.Matches.SingleOrDefault(e =>
            e.LocalTeamId == match.LocalTeamId &&
            e.AwayTeamId == match.AwayTeamId);

        if (currentMatch is null)
            return Results.MatchDoesNotExist;

        if (currentMatch.MatchResult.Played)
            return Results.MatchAlreadyPlayed;

        return Results.Unit;
    }

    public Results MatchIsNotDraw(MatchResult matchResult)
        => matchResult.LocalGoals == matchResult.AwayGoals
            ? Results.DrawResultIsNotAllowed
            : Results.Unit;
}
