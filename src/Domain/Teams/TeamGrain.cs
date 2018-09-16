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
        {
        }

        public Task CreateAsync(CreateTeam cmd)
            => PersistPublish(TeamCreated.From(cmd));

        public Task AddPlayerAsync(AddPlayer cmd)
            => TeamExists(State).Match(
                s => PersistPublish(PlayerAdded.From(cmd)),
                f => Task.CompletedTask);

        public Task<Validation<TeamErrorCodes, TeamState>> GetTeamAsync()
            => Task.FromResult(TeamExists(State).Map(s => State));
    }
}