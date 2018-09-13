using System.Collections.Immutable;
using System.Threading.Tasks;
using Orleans;
using Snaelro.Domain.Teams.Commands;

namespace Snaelro.Domain.Teams.Aggregates
{
    // Commands
    public partial interface ITeamGrain : IGrainWithGuidKey
    {
        Task CreateAsync(CreateTeam cmd);

        Task AddPlayerAsync(AddPlayer cmd);
    }

    // Queries
    public partial interface ITeamGrain : IGrainWithGuidKey
    {
        Task<string> GetNameAsync();

        Task<IImmutableList<string>> GetPlayersAsync();
    }
}