using System.Linq;
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
        private StreamSubscriptionHandle<object> _subscription;

        private readonly StreamOptions _streamOpt;
        protected readonly ILogger _logger;

        protected SubscriberGrain(
            StreamOptions streamOpt,
            ILogger logger)
        {
            _streamOpt = streamOpt;
            _logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            var guid = this.GetPrimaryKey();
            var streamProvider = GetStreamProvider(_streamOpt.Provider);
            var stream = streamProvider.GetStream<object>(guid, _streamOpt.Namespace);

            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            var subs = subscriptionHandles.FirstOrDefault(e => e.HandleId == guid);
            if (subs != null)
            {
                _subscription = subs;
                await subs.ResumeAsync(OnNextAsync);
            }
            else
            {
                _subscription = await stream.SubscribeAsync(OnNextAsync);
            }
        }

        public override Task OnDeactivateAsync()
        {
            _subscription.UnsubscribeAsync();
            return base.OnDeactivateAsync();
        }

        public async Task OnNextAsync(object evt, StreamSequenceToken token)
        {
            var handled = await HandleAsync(evt, token);

            if (handled)
                _logger.LogInformation(
                "handled event of type [{evtType}] for resource id: [{grainId}]", evt.GetType().Name, this.GetPrimaryKey());
        }

        public abstract Task<bool> HandleAsync(object evt, StreamSequenceToken token);
    }
}
