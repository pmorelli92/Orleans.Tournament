using System.Threading.Tasks;
using Orleans;

namespace Snaelro.Domain.Teams.Aggregates
{
    public interface ITeamGrain : IGrainWithGuidKey
    {
        Task<string> EchoAsync(string message);
    }

    public class TeamGrain : Grain, ITeamGrain
    {
        public Task<string> EchoAsync(string message)
        {
            var str = $"[TeamGrain] echoed: {message} from " +
                      $"grain with id: {this.GetPrimaryKey()} " +
                      $"and identity: {IdentityString}";

            return Task.FromResult(str);
        }
    }
}