using Orleans.Streams;

namespace Orleans.Tournament.Domain.Abstractions.Grains;

// This interface is needed for Orleans' Grain activation
public interface ISubscriber : IGrainWithGuidKey
{
}

public abstract class SubscriberGrain : Grain, ISubscriber
{
    private readonly string _type;
    private readonly string _namespace;
    private StreamSubscriptionHandle<object>? _sub;

    protected IStreamProvider? StreamProvider;

    protected SubscriberGrain(StreamConfig streamConfig)
    {
        (_type, _namespace) = streamConfig;
    }

    public async override Task OnActivateAsync()
    {
        // StreamProvider cannot be obtained outside the Orleans lifecycle methods
        StreamProvider = GetStreamProvider(_type);

        _sub = await StreamProvider
            .GetStream<object>(this.GetPrimaryKey(), _namespace)
            .SubscribeAsync(HandleAsync);

        await base.OnActivateAsync();
    }

    public async override Task OnDeactivateAsync()
    {
        await _sub!.UnsubscribeAsync();
        await base.OnDeactivateAsync();
    }

    public abstract Task<bool> HandleAsync(object evt, StreamSequenceToken token);
}