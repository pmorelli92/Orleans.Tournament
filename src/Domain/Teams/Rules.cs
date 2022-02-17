namespace Tournament.Domain.Teams;

public partial class TeamState
{
    public Results TeamExists()
        => Created
            ? Results.Unit
            : Results.TeamDoesNotExist;

    public Results TeamDoesNotExists()
        => !Created
            ? Results.Unit
            : Results.TeamAlreadyExist;
}