using System.Collections.Immutable;
using Orleans;
using Orleans.Streams;
using Tournament.Domain;
using Tournament.Domain.Abstractions;
using Tournament.Domain.Abstractions.Grains;
using Tournament.Domain.Teams;
using Tournament.Projections.Tournaments;

namespace Tournament.Projections.Teams;

[ImplicitStreamSubscription(Constants.TeamNamespace)]
public class TeamSubscriber : SubscriberGrain
{
    private readonly ITournamentQueryHandler _tournamentQueryHandler;
    private readonly ProjectionManager<TeamProjection> _projectionManager;

    public TeamSubscriber(
        PostgresOptions postgresOptions,
        ITournamentQueryHandler tournamentQueryHandler)
        : base(
            new StreamConfig(Constants.InMemoryStream, Constants.TeamNamespace))
    {
        _tournamentQueryHandler = tournamentQueryHandler;
        _projectionManager = new ProjectionManager<TeamProjection>("read", "team_projection", postgresOptions);
    }

    public override async Task<bool> HandleAsync(object evt, StreamSequenceToken token)
    {
        switch (evt)
        {
            case TeamCreated obj:
                return await Handle(obj);

            case PlayerAdded obj:
                return await Handle(obj);

            case TournamentJoined obj:
                return await Handle(obj);

            case ErrorHasOccurred _:
                return true;

            default:
                //PrefixLogger.LogError(
                //    "unhandled event of type [{evtType}] for resource id: [{grainId}]", evt.GetType().Name, this.GetPrimaryKey());
                return false;
        }
    }

    private async Task<bool> Handle(TeamCreated evt)
    {
        var projection = new TeamProjection(
            evt.TeamId,
            evt.Name,
            Enumerable.Empty<string>().ToImmutableList(),
            Enumerable.Empty<Tournament>().ToImmutableList());

        await _projectionManager.UpdateProjection(this.GetPrimaryKey(), projection);
        return true;
    }

    private async Task<bool> Handle(PlayerAdded evt)
    {
        var projection = await _projectionManager.GetProjectionAsync(this.GetPrimaryKey());

        await _projectionManager.UpdateProjection(
            this.GetPrimaryKey(),
            projection with { Players = projection.Players.Add(evt.Name) });

        return true;
    }

    private async Task<bool> Handle(TournamentJoined evt)
    {
        var tournament = await _tournamentQueryHandler.GetTournamentAsync(evt.TournamentId);
        var tournamentObj = new Tournament(tournament.Id, tournament.Name);

        var projection = await _projectionManager.GetProjectionAsync(this.GetPrimaryKey());

        await _projectionManager.UpdateProjection(
            this.GetPrimaryKey(),
            projection with { Tournaments = projection.Tournaments.Add(tournamentObj) });

        return true;
    }
}
