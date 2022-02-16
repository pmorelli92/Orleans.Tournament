using Orleans;
using Tournament.Domain.Abstractions;
using Tournament.Domain.Abstractions.Events;
using Tournament.Domain.Abstractions.Grains;
using Tournament.Domain.Teams;

namespace Tournament.Domain.Tournaments;

public interface ITournamentGrain : IGrainWithGuidKey
{
    // Commands
    Task CreateAsync(CreateTournament cmd);

    Task AddTeamAsync(AddTeam cmd);

    Task StartAsync(StartTournament cmd);

    Task SetMatchResultAsync(SetMatchResult cmd);
}

public class TournamentGrain : EventSourcedGrain<TournamentState>, ITournamentGrain
{
    public TournamentGrain()
        : base(new StreamConfig(Constants.InMemoryStream, Constants.TournamentNamespace))
    { }

    public async Task CreateAsync(CreateTournament cmd)
    {
        var task = State.TournamentDoesNotExists() switch
        {
            Results.Unit => PersistPublishAsync(new TournamentCreated(cmd.Name, cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId)),
            Results x => PublishErrorAsync(new ErrorHasOccurred((int)x, nameof(CreateTournament), cmd.TraceId, cmd.InvokerUserId))
        };

        await task;
    }

    public async Task AddTeamAsync(AddTeam cmd)
    {
        // Check if the team already exists
        var teamGrain = GrainFactory.GetGrain<ITeamGrain>(cmd.TeamId);
        var exists = await teamGrain.TeamExistAsync();
        if (!exists)
            await PublishErrorAsync(new ErrorHasOccurred((int)Results.TeamDoesNotExist, nameof(AddTeam), cmd.TraceId, cmd.InvokerUserId));

        // Check other preconditions
        var result = ResultsUtil.Eval(
            State.TournamentExists(),
            State.TeamIsNotAdded(cmd.TeamId),
            State.LessThanEightTeams());

        var task = result switch
        {
            Results.Unit => PersistPublishAsync(new TeamAdded(cmd.TeamId, cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId)),
            Results x => PublishErrorAsync(new ErrorHasOccurred((int)x, nameof(AddTeam), cmd.TraceId, cmd.InvokerUserId))
        };

        await task;
    }

    public async Task StartAsync(StartTournament cmd)
    {
        var result = ResultsUtil.Eval(
            State.TournamentExists(),
            State.TournamentDidNotStart(),
            State.EightTeamsToStartTournament());

        var task = result switch
        {
            Results.Unit => PersistPublishAsync(new TournamentStarted(cmd.TournamentId, State.Teams, cmd.TraceId, cmd.InvokerUserId)),
            Results x => PublishErrorAsync(new ErrorHasOccurred((int)x, nameof(TournamentStarted), cmd.TraceId, cmd.InvokerUserId))
        };

        await task;
    }

    public async Task SetMatchResultAsync(SetMatchResult cmd)
    {
        var result = ResultsUtil.Eval(
            State.TournamentExists(),
            State.TournamentStarted(),
            State.MatchIsNotDraw(cmd.Match.MatchResult!),
            State.MatchExistsAndIsNotPlayed(cmd.Match));

        var task = result switch
        {
            Results.Unit => PersistPublishAsync(new MatchResultSet(cmd.TournamentId, cmd.Match, cmd.TraceId, cmd.InvokerUserId)),
            Results x => PublishErrorAsync(new ErrorHasOccurred((int)x, nameof(SetMatchResult), cmd.TraceId, cmd.InvokerUserId))
        };

        await task;
    }
}