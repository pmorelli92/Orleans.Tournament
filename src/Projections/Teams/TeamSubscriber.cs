using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams.Events;

namespace Snaelro.Projections.Teams
{
    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class TeamSubscriber : Grain, ISubscriber
    {
        private readonly ProjectionManager<TeamProjection> _projectionManager;

        public TeamSubscriber(PostgresOptions postgresOptions)
        {
            _projectionManager = new ProjectionManager<TeamProjection>("read", "team_projection", postgresOptions);
        }

        public override async Task OnActivateAsync()
        {
            var guid = this.GetPrimaryKey();
            var streamProvider = GetStreamProvider(Constants.TeamStream);
            var stream = streamProvider.GetStream<object>(guid, Constants.StreamNamespace);
            await stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync();
        }

        public Task OnNextAsync(object evt, StreamSequenceToken token = null)
        {
            switch (evt)
            {
                case TeamCreated obj:
                    return Handle(obj);
                case PlayerAdded obj:
                    return Handle(obj);
                default:
                    Console.WriteLine($"[TeamSubscriber] event of type {evt.GetType()} unhandled");
                    return Task.CompletedTask;
            }
        }

        private async Task Handle(TeamCreated evt)
        {
            var projection = TeamProjection.New.SetName(evt.Name);
            await _projectionManager.UpdateProjection(this.GetPrimaryKey(), projection);
            Console.WriteLine("[TeamSubscriber] TeamCreated handled");
        }

        private async Task Handle(PlayerAdded evt)
        {
            var projection =
                (await _projectionManager.GetProjectionAsync(this.GetPrimaryKey()))
                .AddPlayer(evt.Name);

            await _projectionManager.UpdateProjection(this.GetPrimaryKey(), projection);
            Console.WriteLine("[TeamSubscriber] PlayedAdded handled");
        }
    }
}