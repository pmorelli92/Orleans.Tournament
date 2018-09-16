using System.Threading.Tasks;
using LanguageExt;
using Orleans;
using Snaelro.Domain.Teams.Commands;

namespace Snaelro.Domain.Teams
{
    // Commands
    public partial interface ITeamGrain : IGrainWithGuidKey
    {
        Task CreateAsync(CreateTeam cmd);

        Task AddPlayerAsync(AddPlayer cmd);
    }

    // Queries
    public partial interface ITeamGrain
    {
        Task<Validation<TeamErrorCodes, TeamState>> GetTeamAsync();
    }
}