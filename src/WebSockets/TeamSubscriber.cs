using Orleans;
using Orleans.Streams;
using Tournament.Domain;
using Tournament.Domain.Abstractions;
using Tournament.Domain.Abstractions.Grains;

namespace Tournament.WebSockets;

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