using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Abstractions.Grains;
using Constants = Orleans.Tournament.Domain.Helpers;

namespace Orleans.Tournament.WebSockets.Teams
{
    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class TournamentSubscriber : SubscriberGrain
    {
        public TournamentSubscriber(ILogger<TeamSubscriber> logger)
            : base(
                new StreamOptions(Constants.TournamentStream, Constants.StreamNamespace),
                new PrefixLogger(logger, "[Tournament][WebSocket]"))
        {
        }

        public override async Task<bool> HandleAsync(object evt, StreamSequenceToken token = null)
        {
            if (evt is ITraceable obj)
            {
                var streamToPublish = GetStreamProvider("ws");
                var stream = streamToPublish.GetStream<object>(obj.InvokerUserId, Constants.StreamNamespace);
                await stream.OnNextAsync(new WebSocketMessage(evt.GetType().Name, evt));
            }

            return true;
        }
    }
}
