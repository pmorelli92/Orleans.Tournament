using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Abstractions.Grains;
using Constants = Orleans.Tournament.Domain.Helpers;
namespace Orleans.Tournament.WebSockets.Teams
{
    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class TeamSubscriber : SubscriberGrain
    {
        public TeamSubscriber(ILogger<TeamSubscriber> logger)
            : base(
                new StreamOptions(Constants.TeamStream, Constants.StreamNamespace),
                new PrefixLogger(logger, "[Team][WebSocket]"))
        {
        }

        public override async Task HandleAsync(object evt, StreamSequenceToken token = null)
        {
            if (evt is ITraceable obj)
            {
                var streamToPublish = GetStreamProvider("ws");
                var stream = streamToPublish.GetStream<object>(obj.InvokerUserId, Constants.StreamNamespace);
                await stream.OnNextAsync(new WebSocketMessage(evt.GetType().Name, evt));
            }
            else
                PrefixLogger.LogError(
                    "unhandled event of type [{evtType}] for resource id: [{grainId}]", evt.GetType().Name, this.GetPrimaryKey());
        }
    }
}
