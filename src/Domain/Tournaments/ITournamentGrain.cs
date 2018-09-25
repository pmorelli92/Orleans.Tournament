using System.Threading.Tasks;
using LanguageExt;
using Orleans;
using Snaelro.Domain.Tournaments.Commands;

namespace Snaelro.Domain.Tournaments
{
    // Commands
    public partial interface ITournamentGrain : IGrainWithGuidKey
    {
        Task CreateAsync(CreateTournament cmd);

        Task AddTeamAsync(AddTeam cmd);

        Task StartAsync();

        Task SetMatchResultAsync(SetMatchResult cmd);

        Task NextPhaseAsync();
    }

    // Queries
    public partial interface ITournamentGrain
    {
        Task<Validation<TournamentErrorCodes, TournamentState>> GetTournamentAsync();
    }
}