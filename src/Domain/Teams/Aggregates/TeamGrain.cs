using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Streams;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams.Events;
using Snaelro.Domain.Teams.ValueObjects;

namespace Snaelro.Domain.Teams.Aggregates
{
    public class TeamGrain : JournaledGrain<State>, ITeamGrain
    {
        private IAsyncStream<object> _stream;

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.TeamStream);
            _stream = streamProvider.GetStream<object>(this.GetPrimaryKey(), Constants.StreamNamespace);
            await base.OnActivateAsync();
        }

        public async Task<string> EchoAsync(string message)
        {
            var str = $"[TeamGrain] echoed: {message} from " +
                      $"grain with id: {this.GetPrimaryKey()} " +
                      $"and identity: {IdentityString}";

            var evt = new Echoed(str);

            RaiseEvent(evt);
            await _stream.OnNextAsync(evt);

            return str;
        }

        public Task<IEnumerable<string>> GetMessagesAsync()
        {
            return Task.FromResult(State.Messages.AsEnumerable());
        }
    }
}