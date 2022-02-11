using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Abstractions.Events;
using Orleans.Tournament.Domain.Abstractions.Grains;
using Orleans.Tournament.Domain.Teams;
using Orleans.Tournament.Projections.Tournaments;
using Constants = Orleans.Tournament.Domain.Helpers;

namespace Orleans.Tournament.Projections.Teams;

[ImplicitStreamSubscription(Constants.TeamNamespace)]
public class TeamSubscriber : SubscriberGrain
{
    private readonly ITournamentQueryHandler _tournamentQueryHandler;
    private readonly ProjectionManager<TeamProjection> _projectionManager;

    public TeamSubscriber(
        PostgresOptions postgresOptions,
        ITournamentQueryHandler tournamentQueryHandler,
        ILogger<TeamSubscriber> logger)
        : base(
            new StreamOptions(Constants.MemoryProvider, Constants.TeamNamespace),
            logger)
    {
        _tournamentQueryHandler = tournamentQueryHandler;
        _projectionManager = new ProjectionManager<TeamProjection>("read", "team_projection", postgresOptions);
    }

    public override async Task<bool> HandleAsync(object evt, StreamSequenceToken token = null)
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