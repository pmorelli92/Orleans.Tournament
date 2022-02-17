using Orleans;
using Orleans.EventSourcing;
using Orleans.Streams;

namespace Tournament.Domain.Abstractions.Grains;

public abstract class EventSourcedGrain<TState> : JournaledGrain<TState>
    where TState : class, new()
{
    private readonly string _type;
    private readonly string _namespace;
    protected IStreamProvider? StreamProvider;

    protected EventSourcedGrain(StreamConfig streamConfig)
    {
        (_type, _namespace) = streamConfig;
    }

    public override Task OnActivateAsync()
    {
        // StreamProvider cannot be obtained outside the Orleans lifecycle methods
        StreamProvider = GetStreamProvider(_type);

        return base.OnActivateAsync();
    }

    protected async Task PersistPublishAsync(object evt)
    {
        RaiseEvent(evt);

        await StreamProvider!
            .GetStream<object>(this.GetPrimaryKey(), _namespace)
            .OnNextAsync(evt);
    }

    protected async Task PublishErrorAsync(ErrorHasOccurred evt)
    {
        await StreamProvider!
            .GetStream<object>(this.GetPrimaryKey(), _namespace)
            .OnNextAsync(evt);
    }
}