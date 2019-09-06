using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Abstractions.Grains;
using Orleans.Tournament.Domain;
using Orleans.Tournament.Domain.Teams.Events;
using Orleans.Tournament.Projections.Tournaments;

namespace Orleans.Tournament.Projections.Teams
{
    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class TeamSubscriber : SubscriberGrain
    {
        private readonly ITournamentQueryHandler _tournamentQueryHandler;
        private readonly ProjectionManager<TeamProjection> _projectionManager;

        public TeamSubscriber(
            PostgresOptions postgresOptions,
            ITournamentQueryHandler tournamentQueryHandler,
            ILogger<TeamSubscriber> logger)
            : base(
                new StreamOptions(Constants.TeamStream, Constants.StreamNamespace),
                new PrefixLogger(logger, "[Team][Projection]"))
        {
            _tournamentQueryHandler = tournamentQueryHandler;
            _projectionManager = new ProjectionManager<TeamProjection>("read", "team_projection", postgresOptions);
        }

        public override async Task HandleAsync(object evt, StreamSequenceToken token = null)
        {
            switch (evt)
            {
                case TeamCreated obj:
                    await Handle(obj);
                    break;
                case PlayerAdded obj:
                    await Handle(obj);
                    break;
                case TournamentJoined obj:
                    await Handle(obj);
                    break;
                default:
                    PrefixLogger.LogError(
                        "unhandled event of type [{evtType}] for resource id: [{grainId}]", evt.GetType().Name, this.GetPrimaryKey());
                    break;
            }
        }

        private async Task Handle(TeamCreated evt)
        {
            var projection = TeamProjection.New.SetName(evt.TeamId, evt.Name);
            await _projectionManager.UpdateProjection(this.GetPrimaryKey(), projection);
        }

        private async Task Handle(PlayerAdded evt)
        {
            var projection = await _projectionManager.GetProjectionAsync(this.GetPrimaryKey());
            await _projectionManager.UpdateProjection(this.GetPrimaryKey(), projection.AddPlayer(evt.Name));
        }

        private async Task Handle(TournamentJoined evt)
        {
            var tournament = await _tournamentQueryHandler.GetTournamentAsync(evt.TournamentId);
            var tournamentObj = new TeamProjection.Tournament(tournament.Id, tournament.Name);

            var projection = await _projectionManager.GetProjectionAsync(this.GetPrimaryKey());
            await _projectionManager.UpdateProjection(this.GetPrimaryKey(), projection.JoinTournament(tournamentObj));
        }
    }
}
