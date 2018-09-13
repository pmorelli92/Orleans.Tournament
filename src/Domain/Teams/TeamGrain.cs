using System.Collections.Immutable;
using System.Threading.Tasks;
using LanguageExt;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Streams;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams.Commands;
using Snaelro.Domain.Teams.Events;
using Snaelro.Domain.Teams.ValueObjects;
using static Snaelro.Domain.Teams.TeamRules;

namespace Snaelro.Domain.Teams
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

        private Task PersistPublish(object evt)
        {
            RaiseEvent(evt);
            return _stream.OnNextAsync(evt);
        }

        public Task CreateAsync(CreateTeam cmd)
            => PersistPublish(new TeamCreated(cmd.Name));

        public Task AddPlayerAsync(AddPlayer cmd)
            => TeamExists(State).Match(
                s => PersistPublish(new PlayerAdded(cmd.Name)),
                f => Task.CompletedTask);

        public Task<Validation<TeamErrorCodes, string>> GetNameAsync()
            => Task.FromResult(TeamExists(State).Map(s => State.Name));

        public Task<Validation<TeamErrorCodes, IImmutableList<string>>> GetPlayersAsync()
            => Task.FromResult(TeamExists(State).Map(s => State.Players));
    }
}