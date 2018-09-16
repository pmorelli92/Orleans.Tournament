using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using Snaelro.Domain;
using Snaelro.Domain.Snaelro.Domain;

namespace Snaelro.Sockets.Teams
{
    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class TeamSubscriber : Subscriber
    {
        public TeamSubscriber()
            : base(Constants.TeamStream, Constants.StreamNamespace)
        {
        }

        public override async Task OnNextAsync(object evt, StreamSequenceToken token = null)
        {
            if (evt is ITraceable obj)
            {
                var streamToPublish = GetStreamProvider("ws");
                var stream = streamToPublish.GetStream<object>(obj.TraceId, Constants.StreamNamespace);
                await stream.OnNextAsync(new WebSocketMessage(evt.GetType().Name, evt));
                Console.WriteLine($"[Team][WebSocket] published event with TraceId: {obj.TraceId}]");
            }
            else
                Console.WriteLine($"[Team][WebSocket] event of type {evt.GetType()} unhandled");
        }
    }

    public class WebSocketMessage
    {
        public string Type { get; set; }

        public object Payload { get; set; }

        public WebSocketMessage(string type, object payload)
        {
            Type = type;
            Payload = payload;
        }
    }
}