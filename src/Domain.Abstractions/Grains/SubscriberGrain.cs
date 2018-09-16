using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;

namespace Snaelro.Domain.Abstractions.Grains
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

        public async Task OnNextAsync(object evt, StreamSequenceToken token = null)
        {
            await HandleAsync(evt, token);

            PrefixLogger.LogInformation(
                "handled event of type {evtType} for resource id: {grainId}", evt.GetType(), this.GetPrimaryKey());
        }

        public abstract Task HandleAsync(object evt, StreamSequenceToken token = null);
    }
}