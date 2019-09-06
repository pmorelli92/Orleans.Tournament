namespace Orleans.Tournament.Domain
{
    public enum BusinessErrors
    {
        TeamDoesNotExist = 1,
        TeamAlreadyExist = 2,
        TournamentDoesNotExist = 3,
        TournamentAlreadyExist = 4,
        TeamIsAlreadyAdded = 5,
        TournamentHasMoreThanEightTeams = 6,
        CantStartTournamentWithLessThanEightTeams = 7,
        TournamentIsNotStarted = 8,
        MatchDoesNotExist = 9,
        MatchAlreadyPlayed = 10,
        DrawResultIsNotAllowed = 11,
        NotAllMatchesPlayed = 12,
        TournamentAlreadyOnFinals = 13,
        TournamentAlreadyStarted = 14,
    }
}
