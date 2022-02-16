namespace Tournament.API.Tournaments;

public record AddTeamModel(Guid TeamId);

public record CreateTournamentModel(string Name);

public record SetMatchResultModel(
        Guid LocalTeamId,
        int LocalGoals,
        Guid AwayTeamId,
        int AwayGoals);