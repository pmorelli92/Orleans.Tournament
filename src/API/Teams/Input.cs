namespace Tournament.API.Teams;

public record CreateTeamModel(string Name);

public record AddPlayerModel(IEnumerable<string> Names);