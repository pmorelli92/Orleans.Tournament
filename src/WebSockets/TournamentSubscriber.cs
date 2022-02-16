using Orleans;
using Orleans.Streams;
using Tournament.Domain;
using Tournament.Domain.Abstractions;
using Tournament.Domain.Abstractions.Grains;

namespace Tournament.WebSockets;

// Subscribes to the InMemoryStream for the TournamentNamespace
// Each event will be then published back on the stream but on the WebSocketNamespace
[ImplicitStreamSubscription(Constants.TournamentNamespace)]
public class TournamentSubscriber : SubscriberGrain
{
    public TournamentSubscriber()
        : base(new StreamConfig(Constants.InMemoryStream, Constants.TournamentNamespace))
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