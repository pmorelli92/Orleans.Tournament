using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams.Events;

namespace Snaelro.Projections.Teams
{
    public interface IRandomReceiver : IGrainWithGuidKey
    {
        Task DoSmth();
    }

    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class Subscriber : Grain, IRandomReceiver
    {
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
                Console.WriteLine($"Received: {this.GetPrimaryKey()} message: {echoed.Message}");
            }

            return Task.CompletedTask;
        }

        public Task DoSmth()
        {
            throw new NotImplementedException();
        }
    }
}