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
        private readonly Options.Postgres _postgresOptions;

        public TeamSubscriber(Options.Postgres postgresOptions)
            => _postgresOptions = postgresOptions;

        public override async Task OnActivateAsync()
        {
            var guid = this.GetPrimaryKey();
            var streamProvider = GetStreamProvider(Constants.TeamStream);
            var stream = streamProvider.GetStream<object>(guid, Constants.StreamNamespace);
            await stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync();
        }

        public Task OnNextAsync(object item, StreamSequenceToken token = null)
        {
            if (item is Echoed echoed)
            {
                Console.WriteLine($"Postgres opt: {_postgresOptions.ConnectionString}");
                Console.WriteLine($"Received: {this.GetPrimaryKey()} message: {echoed.Message}");
            }

            return Task.CompletedTask;
        }
    }
}