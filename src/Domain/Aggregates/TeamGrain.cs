using System.Threading.Tasks;
using Orleans;

namespace Snaelro.Domain.Aggregates
{
    public interface ITeamGrain : IGrainWithGuidKey
    {
        Task<string> HelloWorldAsync();
    }

    public class TeamGrain : Grain, ITeamGrain
    {
        public Task<string> HelloWorldAsync()
            => Task.FromResult("Hello");
    }
}