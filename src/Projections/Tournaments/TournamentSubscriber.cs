using System.Collections.Immutable;
using Orleans;
using Orleans.Streams;
using Tournament.Domain;
using Tournament.Domain.Abstractions;
using Tournament.Domain.Abstractions.Events;
using Tournament.Domain.Abstractions.Grains;
using Tournament.Domain.Tournaments;
using Tournament.Projections.Teams;

namespace Tournament.Projections.Tournaments;

[ImplicitStreamSubscription(Constants.TournamentNamespace)]
public class TournamentSubscriber : SubscriberGrain
{
    private readonly ITeamQueryHandler _teamQueryHandler;
    private readonly ProjectionManager<TournamentProjection> _projectionManager;

    public TournamentSubscriber(
        PostgresOptions postgresOptions,
        ITeamQueryHandler teamQueryHandler)
        : base(
            new StreamConfig(Constants.InMemoryStream, Constants.TournamentNamespace))
    {
        _projectionManager = new ProjectionManager<TournamentProjection>("read", "tournament_projection", postgresOptions);
        _teamQueryHandler = teamQueryHandler;
    }

    public override async Task<bool> HandleAsync(object evt, StreamSequenceToken token = null)
    {
        switch (evt)
        {
            case TournamentCreated obj:
                return await Handle(obj);

            case TeamAdded obj:
                return await Handle(obj);

            case TournamentStarted obj:
                return await Handle(obj);

            case MatchResultSet obj:
                return await Handle(obj);

            case ErrorHasOccurred _:
                return true;

            default:
                //logger.LogError(
                //    "unhandled event of type [{evtType}] for resource id: [{grainId}]", evt.GetType().Name, this.GetPrimaryKey());
                return false;
        }
    }

    private async Task<bool> Handle(TournamentCreated evt)
    {
        var projection = new TournamentProjection(
            evt.TournamentId,
            evt.Name,
            Enumerable.Empty<Team>().ToImmutableList(),
            null); //TODO: Fix this

        await _projectionManager.UpdateProjection(this.GetPrimaryKey(), projection);
        return true;
    }

    private async Task<bool> Handle(TeamAdded evt)
    {
        var team = await _teamQueryHandler.GetTeamAsync(evt.TeamId);
        var teamObj = new Team(team.Id, team.Name);

        var projection = await _projectionManager.GetProjectionAsync(this.GetPrimaryKey());

        await _projectionManager.UpdateProjection(
            this.GetPrimaryKey(),
            projection with { Teams = projection.Teams.Add(teamObj) });

        return true;
    }

    // The idea is to serve the data in a friendly way to the user, in this case
    // I am using the same value object that the state is using but it can be changed here
    private async Task<bool> Handle(TournamentStarted evt)
    {
        var projection = await _projectionManager.GetProjectionAsync(this.GetPrimaryKey());

        await _projectionManager.UpdateProjection(
            this.GetPrimaryKey(),
            projection with { Fixture = Fixture.Create(evt.Teams) });

        return true;
    }

    private async Task<bool> Handle(MatchResultSet evt)
    {
        var projection = await _projectionManager.GetProjectionAsync(this.GetPrimaryKey());

        await _projectionManager.UpdateProjection(
            this.GetPrimaryKey(),
            projection with { Fixture = projection.Fixture.SetMatchResult(evt.Match) });

        return true;
    }
}