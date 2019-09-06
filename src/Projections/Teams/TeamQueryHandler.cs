using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Tournament.Projections.Teams
{
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

        public async Task<TeamProjection> GetTeamAsync(Guid id)
        {
            var asd = await _projectionManager.GetProjectionAsync(id);
            return asd;
        }

        public Task<IReadOnlyList<TeamProjection>> GetTeamsAsync()
            => _projectionManager.GetProjectionsAsync();
    }
}
