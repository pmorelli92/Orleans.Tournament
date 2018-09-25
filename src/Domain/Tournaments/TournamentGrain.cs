using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Abstractions.Grains;
using Snaelro.Domain.Snaelro.Domain;
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
            => await TournamentExists(State).Match(
                s => PersistPublish(TeamAdded.From(cmd)),
                f => Task.CompletedTask);

        public Task StartAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task SetMatchResultAsync(SetMatchResult cmd)
        {
            throw new System.NotImplementedException();
        }

        public Task NextPhaseAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Validation<TournamentErrorCodes, TournamentState>> GetTournamentAsync()
            => Task.FromResult(TournamentExists(State).Map(s => State));
    }
}