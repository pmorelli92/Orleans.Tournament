using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Abstractions.Grains;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams.Commands;
using Snaelro.Domain.Teams.Events;
using static Snaelro.Domain.Teams.TeamRules;

namespace Snaelro.Domain.Teams
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

        public Task<bool> TeamExistAsync()
            => Task.FromResult(State.Created);
    }
}