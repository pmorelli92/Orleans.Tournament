using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Tournament.Projections.Tournaments
{
    public interface ITournamentQueryHandler
    {
        Task<TournamentProjection> GetTournamentAsync(Guid id);

        Task<IReadOnlyList<TournamentProjection>> GetTournamentsAsync();
    }

    public class TournamentQueryHandler : ITournamentQueryHandler
    {
        private readonly ProjectionManager<TournamentProjection> _projectionManager;

        public TournamentQueryHandler(PostgresOptions postgresOptions)
        {
            _projectionManager = new ProjectionManager<TournamentProjection>("read", "tournament_projection", postgresOptions);
        }

        public Task<TournamentProjection> GetTournamentAsync(Guid id)
            => _projectionManager.GetProjectionAsync(id);

        public Task<IReadOnlyList<TournamentProjection>> GetTournamentsAsync()
            => _projectionManager.GetProjectionsAsync();
    }
}
