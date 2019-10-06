using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace Orleans.Tournament.Domain.Abstractions.Grains
{
    public interface ISubscriber : IGrainWithGuidKey
    {
    }

    public abstract class SubscriberGrain : Grain, ISubscriber
    {
        private readonly StreamOptions _streamOpt;
        protected readonly PrefixLogger PrefixLogger;

        protected SubscriberGrain(
            StreamOptions streamOpt,
            PrefixLogger prefixLogger)
        {
            _streamOpt = streamOpt;
            PrefixLogger = prefixLogger;
        }

        public override async Task OnActivateAsync()
        {
            var guid = this.GetPrimaryKey();
            var streamProvider = GetStreamProvider(_streamOpt.Name);
            var stream = streamProvider.GetStream<object>(guid, _streamOpt.Namespace);
            await stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync();
        }

        public async Task OnNextAsync(object evt, StreamSequenceToken token)
        {
            await HandleAsync(evt, token);

            // TODO: Do we always want to log the events if the subscriber is not acting on it (for example Tournament Saga)
            PrefixLogger.LogInformation(
                "handled event of type [{evtType}] for resource id: [{grainId}]", evt.GetType().Name, this.GetPrimaryKey());
        }

        public abstract Task HandleAsync(object evt, StreamSequenceToken token);
    }
}
