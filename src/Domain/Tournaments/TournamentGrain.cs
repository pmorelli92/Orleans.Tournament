using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Orleans;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Abstractions.Grains;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams;
using Snaelro.Domain.Tournaments.Commands;
using Snaelro.Domain.Tournaments.Events;
using Snaelro.Domain.Tournaments.ValueObject;
using static Snaelro.Domain.Tournaments.TournamentRules;

namespace Snaelro.Domain.Tournaments
{
    public class TournamentGrain : EventSourcedGrain<TournamentState>, ITournamentGrain
    {
        public TournamentGrain(ILogger<TournamentGrain> logger)
            : base(
                new StreamOptions(Constants.TournamentStream, Constants.StreamNamespace),
                new PrefixLogger(logger, "[Tournament][Grain]"))
        {
        }

        private Task EmitErrorsAsync(IEnumerable<BusinessErrors> f, Guid traceId, Guid invokerUserId)
            => f.Iter(async e => await PublishErrorAsync((int) e, e.ToString(), traceId, invokerUserId)).AsTask();

        public async Task CreateAsync(CreateTournament cmd)
        {
            await TournamentDoesNotExists(State).Match(
                async s => await PersistPublishAsync(TournamentCreated.From(cmd)),
                async f => await EmitErrorsAsync(f, cmd.TraceId, cmd.InvokerUserId));
        }

        public async Task AddTeamAsync(AddTeam cmd)
        {
            // There is no problem on validating on the controller using the
            // read side tables, but this validation should be done on the domain
            // as this is the entry point of truth
            var teamId = GrainFactory.GetGrain<ITeamGrain>(cmd.TeamId);
            var teamExist = await teamId.TeamExistAsync();

            await (TournamentExists(State) |
                   TeamExistsForward(teamExist) |
                   TeamIsNotAdded(State, cmd.TeamId) |
                   LessThanEightTeams(State)).Match(
                async s => await PersistPublishAsync(TeamAdded.From(cmd)),
                async f => await EmitErrorsAsync(f, cmd.TraceId, cmd.InvokerUserId));
        }

        public async Task StartAsync(StartTournament cmd)
        {
            await (TournamentExists(State) |
                   TournamentIsNotStarted(State) |
                   EightTeamsToStartTournament(State)).Match(
                async s =>
                {
                    var shuffledTeams = State.Teams.Shuffle().ToImmutableList();
                    await PersistPublishAsync(TournamentStarted.From(cmd, shuffledTeams));
                },
                async f => await EmitErrorsAsync(f, cmd.TraceId, cmd.InvokerUserId));
        }

        public async Task SetMatchResultAsync(SetMatchResult cmd)
        {
            await (TournamentExists(State) |
                   TournamentIsStarted(State) |
                   MatchIsNotDraw(cmd.MatchResult) |
                   TournamentMatchExistsAndIsNotPlayed(State, cmd.MatchResult)).Match(
                async s => await PersistPublishAsync(MatchResultSet.From(cmd)),
                async f => await EmitErrorsAsync(f, cmd.TraceId, cmd.InvokerUserId));
        }

        public async Task NextPhaseAsync(StartNextPhase cmd)
        {
            await (TournamentExists(State) |
                   TournamentIsStarted(State) |
                   TournamentPhaseCompleted(State) |
                   TournamentIsNotOnFinals(State)).Match(
                async s => await PersistPublishAsync(NextPhaseStarted.From(cmd)),
                async f => await EmitErrorsAsync(f, cmd.TraceId, cmd.InvokerUserId));
        }
    }
}