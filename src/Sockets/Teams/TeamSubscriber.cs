using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using Snaelro.Domain;
using Snaelro.Domain.Snaelro.Domain;

namespace Snaelro.Sockets.Teams
{
    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class TeamSubscriber : Grain, ISubscriber
    {
        public override async Task OnActivateAsync()
        {
            var guid = this.GetPrimaryKey();
            var streamProvider = GetStreamProvider(Constants.TeamStream);
            var stream = streamProvider.GetStream<object>(guid, Constants.StreamNamespace);
            await stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync();
        }

        public async Task OnNextAsync(object evt, StreamSequenceToken token = null)
        {
            if (evt is ITraceable obj)
            {
                var streamToPublish = GetStreamProvider("ws");
                var stream = streamToPublish.GetStream<object>(obj.TraceId, Constants.StreamNamespace);
                await stream.OnNextAsync(evt);
                Console.WriteLine($"[Team][WebSocket] published event with TraceId: {obj.TraceId}]");
            }
            else
                Console.WriteLine($"[Team][WebSocket] event of type {evt.GetType()} unhandled");
        }
    }
}