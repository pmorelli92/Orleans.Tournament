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
using static Snaelro.Domain.Tournaments.TournamentRules;

namespace Snaelro.Domain.Tournaments
{
    public class TournamentGrain : EventSourcedGrain<TournamentState>, ITournamentGrain
    {
        public TournamentGrain(ILogger<TournamentGrain> logger)
            : base(
                new StreamOptions(Constants.TeamStream, Constants.StreamNamespace),
                new PrefixLogger(logger, "[Team][Grain]"))
        {
        }

        public async Task CreateAsync(CreateTournament cmd)
            => await PersistPublish(TournamentCreated.From(cmd));

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
                s => PersistPublish(TeamAdded.From(cmd)),
                f => Task.CompletedTask);
        }

        public Task StartAsync(StartTournament cmd)
        {
            return (TournamentExists(State) |
                    TournamentIsNotStarted(State) |
                    EightTeamsToStartTournament(State)).Match(
                s => PersistPublish(TournamentStarted.From(cmd)),
                f => Task.CompletedTask);
        }

        public Task SetMatchResultAsync(SetMatchResult cmd)
        {
            return (TournamentExists(State) |
                    TournamentIsStarted(State) |
                    MatchIsNotDraw(cmd.MatchResult) |
                    TournamentMatchExists(State, cmd.MatchResult)).Match(
                s => PersistPublish(MatchResultSet.From(cmd)),
                f => Task.CompletedTask);
        }

        public Task NextPhaseAsync(StartNextPhase cmd)
        {
            return (TournamentExists(State) |
                    TournamentIsStarted(State) |
                    TournamentPhaseCompleted(State) |
                    TournamentIsNotOnFinals(State)).Match(
                s => PersistPublish(NextPhaseStarted.From(cmd)),
                f => Task.CompletedTask);
        }

        public Task<Validation<TournamentErrorCodes, TournamentState>> GetTournamentAsync()
            => Task.FromResult(TournamentExists(State).Map(s => State));
    }
}