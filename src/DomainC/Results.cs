namespace Tournament.Domain;

public enum Results
{
    Unit = 0,
    TeamDoesNotExist = 1,
    TeamAlreadyExist = 2,
    TournamentDoesNotExist = 3,
    TournamentAlreadyExist = 4,
    TeamIsAlreadyAdded = 5,
    TournamentHasMoreThanEightTeams = 6,
    TournamentCantStartWithLessThanEightTeams = 7,
    TournamentDidNotStart = 8,
    MatchDoesNotExist = 9,
    MatchAlreadyPlayed = 10,
    DrawResultIsNotAllowed = 11,
    NotAllMatchesPlayed = 12,
    TournamentAlreadyOnFinals = 13,
    TournamentAlreadyStarted = 14
}

public class ResultsUtil
{
    public static Results Eval(params Results[] list)
    {
        foreach (var result in list)
            if (result != Results.Unit)
                return result;

        return Results.Unit;
    }
}