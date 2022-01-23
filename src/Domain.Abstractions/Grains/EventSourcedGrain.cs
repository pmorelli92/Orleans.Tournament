using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Streams;
using Orleans.Tournament.Domain.Abstractions.Events;

namespace Orleans.Tournament.Domain.Abstractions.Grains
{
    public abstract class EventSourcedGrain<TState> : JournaledGrain<TState>
        where TState : class, new()
    {
        private IAsyncStream<object> _stream;

        private readonly StreamOptions _streamOpt;
        private readonly ILogger _logger;

        protected EventSourcedGrain(
            StreamOptions streamOpt,
            ILogger logger)
        {
            _streamOpt = streamOpt;
            _logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(_streamOpt.Provider);
            _stream = streamProvider.GetStream<object>(this.GetPrimaryKey(), _streamOpt.Namespace);
            await base.OnActivateAsync();
        }

        protected async Task PersistPublishAsync(object evt)
        {
            RaiseEvent(evt);

            _logger.LogInformation(
                "handled event of type [{evtType}] for resource id: [{grainId}]", evt.GetType().Name, this.GetPrimaryKey());

            await _stream.OnNextAsync(evt);
        }

        protected async Task PublishErrorAsync(int code, string name, Guid traceId, Guid invokerUserId)
        {
            _logger.LogInformation(
                "handled error [{code}]-[{name}] for resource id: [{grainId}]", code, name, this.GetPrimaryKey());

            await _stream.OnNextAsync(new ErrorHasOccurred(code, name, traceId, invokerUserId));
        }
    }
}
