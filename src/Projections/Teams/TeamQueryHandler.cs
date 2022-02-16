namespace Tournament.Projections.Teams;

public interface ITeamQueryHandler
{
    Task<TeamProjection> GetTeamAsync(Guid id);

    Task<IReadOnlyList<TeamProjection>> GetTeamsAsync();
}

public class TeamQueryHandler : ITeamQueryHandler
{
    private readonly ProjectionManager<TeamProjection> _projectionManager;

    public TeamQueryHandler(PostgresOptions postgresOptions)
    {
        _projectionManager = new ProjectionManager<TeamProjection>("read", "team_projection", postgresOptions);
    }

    public Task<TeamProjection> GetTeamAsync(Guid id)
        => _projectionManager.GetProjectionAsync(id);

    public Task<IReadOnlyList<TeamProjection>> GetTeamsAsync()
        => _projectionManager.GetProjectionsAsync();
}