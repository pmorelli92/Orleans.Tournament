using System.Threading.Tasks;
using LanguageExt;
using Orleans;
using Orleans.Tournament.Domain.Tournaments.Commands;

namespace Orleans.Tournament.Domain.Tournaments
{
    // Commands
    public partial interface ITournamentGrain : IGrainWithGuidKey
    {
        Task CreateAsync(CreateTournament cmd);

        Task AddTeamAsync(AddTeam cmd);

        Task StartAsync(StartTournament cmd);

        Task SetMatchResultAsync(SetMatchResult cmd);

        Task NextPhaseAsync(StartNextPhase cmd);
    }

    // Queries
    public partial interface ITournamentGrain
    {
    }
}
