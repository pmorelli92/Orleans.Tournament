using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace Snaelro.Domain.Teams.Aggregates
{
    public interface ITeamGrain : IGrainWithGuidKey
    {
        Task<string> EchoAsync(string message);

        Task<IEnumerable<string>> GetMessagesAsync();
    }
}