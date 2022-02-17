using Orleans;
using Tournament.Domain.Abstractions;
using Tournament.Domain.Abstractions.Grains;

namespace Tournament.Domain.Teams;

public interface ITeamGrain : IGrainWithGuidKey
{
    // Commands
    Task CreateAsync(CreateTeam cmd);

    Task AddPlayerAsync(AddPlayer cmd);

    Task JoinTournamentAsync(JoinTournament cmd);

    // Queries
    Task<bool> TeamExistAsync();
}

public class TeamGrain : EventSourcedGrain<TeamState>, ITeamGrain
{
    public TeamGrain()
        : base(new StreamConfig(Constants.InMemoryStream, Constants.TeamNamespace))
    { }

    public async Task CreateAsync(CreateTeam cmd)
    {
        var task = State.TeamDoesNotExists() switch
        {
            Results.Unit => PersistPublishAsync(new TeamCreated(cmd.Name, cmd.TeamId, cmd.TraceId, cmd.InvokerUserId)),
            Results x => PublishErrorAsync(new ErrorHasOccurred((int)x, x.ToString(), cmd.TraceId, cmd.InvokerUserId))
        };

        await task;
    }

    public async Task AddPlayerAsync(AddPlayer cmd)
    {
        var task = State.TeamExists() switch
        {
            Results.Unit => PersistPublishAsync(new PlayerAdded(cmd.Name, cmd.TeamId, cmd.TraceId, cmd.InvokerUserId)),
            Results x => PublishErrorAsync(new ErrorHasOccurred((int)x, x.ToString(), cmd.TraceId, cmd.InvokerUserId))
        };

        await task;
    }

    // Saga command, already validated in the saga
    public async Task JoinTournamentAsync(JoinTournament cmd)
        => await PersistPublishAsync(new TournamentJoined(cmd.TeamId, cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId));

    public Task<bool> TeamExistAsync()
        => Task.FromResult(State.Created);
}