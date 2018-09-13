using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Streams;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams.Commands;
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

        public async Task CreateAsync(CreateTeam cmd)
        {
            var evt = new TeamCreated(cmd.Name);

            RaiseEvent(evt);
            await _stream.OnNextAsync(evt);
        }

        public async Task AddPlayerAsync(AddPlayer cmd)
        {
            if (State.Created)
            {
                var evt = new PlayerAdded(cmd.Name);

                RaiseEvent(evt);
                await _stream.OnNextAsync(evt);
            }
        }

        public Task<string> GetNameAsync()
        {
            return Task.FromResult(
                State.Created == false
                    ? "TODO: Error2"
                    : State.Name);
        }

        public Task<IImmutableList<string>> GetPlayersAsync()
        {
            return Task.FromResult(
                State.Created == false
                    ? null
                    : State.Players);
        }
    }
}