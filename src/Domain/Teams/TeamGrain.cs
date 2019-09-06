using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Abstractions.Grains;
using Orleans.Tournament.Domain;
using Orleans.Tournament.Domain.Teams.Commands;
using Orleans.Tournament.Domain.Teams.Events;
using static Orleans.Tournament.Domain.Teams.TeamRules;

namespace Orleans.Tournament.Domain.Teams
{
    public class TeamGrain : EventSourcedGrain<TeamState>, ITeamGrain
    {
        public TeamGrain(ILogger<TeamGrain> logger)
            : base(
                new StreamOptions(Constants.TeamStream, Constants.StreamNamespace),
                new PrefixLogger(logger, "[Team][Grain]"))
        { }

        private Task EmitErrorsAsync(IEnumerable<BusinessErrors> f, Guid traceId, Guid invokerUserId)
            => f.Iter(async e => await PublishErrorAsync((int) e, e.ToString(), traceId, invokerUserId)).AsTask();

        public async Task CreateAsync(CreateTeam cmd)
        {
            await TeamDoesNotExists(State).Match(
                async s => await PersistPublishAsync(TeamCreated.From(cmd)),
                async f => await EmitErrorsAsync(f, cmd.TraceId, cmd.InvokerUserId));
        }

        public async Task AddPlayerAsync(AddPlayer cmd)
        {
            await TeamExists(State).Match(
                async s => await PersistPublishAsync(PlayerAdded.From(cmd)),
                async f => await EmitErrorsAsync(f, cmd.TraceId, cmd.InvokerUserId));
        }

        // Saga command
        public async Task JoinTournamentAsync(JoinTournament cmd)
            => await PersistPublishAsync(TournamentJoined.From(cmd));

        public Task<bool> TeamExistAsync()
            => Task.FromResult(State.Created);
    }
}
