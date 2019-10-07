using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Abstractions.Grains;
using Constants = Orleans.Tournament.Domain.Helpers;
namespace Orleans.Tournament.WebSockets
{
    [ImplicitStreamSubscription(Constants.TeamNamespace)]
    public class TeamSubscriber : SubscriberGrain
    {
        public TeamSubscriber(ILogger<TeamSubscriber> logger)
            : base(
                new StreamOptions(Constants.MemoryProvider, Constants.TeamNamespace),
                new PrefixLogger(logger, "[Team][WebSocket]"))
        {
        }

        public override async Task<bool> HandleAsync(object evt, StreamSequenceToken token = null)
        {
            if (evt is ITraceable obj)
            {
                var streamToPublish = GetStreamProvider(Constants.MemoryProvider);
                var stream = streamToPublish.GetStream<object>(obj.InvokerUserId, Constants.WebSocketNamespace);
                await stream.OnNextAsync(new WebSocketMessage(evt.GetType().Name, evt));
            }

            return true;
        }
    }
}
