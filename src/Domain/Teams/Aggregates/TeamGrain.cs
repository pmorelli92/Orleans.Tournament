using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.EventSourcing;
using Snaelro.Domain.Teams.Events;
using Snaelro.Domain.Teams.ValueObjects;

namespace Snaelro.Domain.Teams.Aggregates
{
    public class TeamGrain : JournaledGrain<State>, ITeamGrain
    {
        public Task<string> EchoAsync(string message)
        {
            var str = $"[TeamGrain] echoed: {message} from " +
                      $"grain with id: {this.GetPrimaryKey()} " +
                      $"and identity: {IdentityString}";

            RaiseEvent(new Echoed(str));
            return Task.FromResult(str);
        }

        public Task<IEnumerable<string>> GetMessagesAsync()
        {
            return Task.FromResult(State.Messages.AsEnumerable());
        }
    }
}