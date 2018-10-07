using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Abstractions.Grains;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams.Events;

namespace Snaelro.Projections.Teams
{
    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class TeamSubscriber : SubscriberGrain
    {
        private readonly ProjectionManager<TeamProjection> _projectionManager;

        public TeamSubscriber(PostgresOptions postgresOptions, ILogger<TeamSubscriber> logger)
            : base(
                new StreamOptions(Constants.TeamStream, Constants.StreamNamespace),
                new PrefixLogger(logger, "[Team][Projection]"))
        {
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
            var projection = await _projectionManager.GetProjectionAsync(this.GetPrimaryKey());
            await _projectionManager.UpdateProjection(this.GetPrimaryKey(), projection.JoinTournament(evt.TournamentId));
        }
    }
}