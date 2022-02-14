using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Abstractions.Grains;
using Constants = Orleans.Tournament.Domain.Helpers;

namespace Orleans.Tournament.WebSockets;

// Subscribes to the InMemoryStream for the TeamNamespace
// Each event will be then published back on the stream but on the WebSocketNamespace
[ImplicitStreamSubscription(Constants.TeamNamespace)]
public class TeamSubscriber : SubscriberGrain
{
    public TeamSubscriber()
        : base(new StreamConfig(Constants.InMemoryStream, Constants.TeamNamespace))
    {
    }

    public override async Task<bool> HandleAsync(object evt, StreamSequenceToken token = null)
    {
        if (evt is ITraceable obj)
            await StreamProvider!
                .GetStream<WebSocketMessage>(obj.InvokerUserId, Constants.WebSocketNamespace)
                .OnNextAsync(new WebSocketMessage(evt.GetType().Name, evt));

        return true;
    }
}