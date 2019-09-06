using System.Threading.Tasks;
using LanguageExt;
using Orleans;
using Orleans.Tournament.Domain.Teams.Commands;

namespace Orleans.Tournament.Domain.Teams
{
    // Commands
    public partial interface ITeamGrain : IGrainWithGuidKey
    {
        Task CreateAsync(CreateTeam cmd);

        Task AddPlayerAsync(AddPlayer cmd);

        Task JoinTournamentAsync(JoinTournament cmd);
    }

    // Queries
    public partial interface ITeamGrain
    {
        Task<bool> TeamExistAsync();
    }
}
