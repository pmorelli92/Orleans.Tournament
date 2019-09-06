using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace Orleans.Tournament.Domain.Abstractions
{
    public class PrefixLogger : ILogger
    {
        private readonly ILogger _logger;

        private readonly string _messagePrefix;

        public PrefixLogger(ILogger logger, string messagePrefix)
        {
            _logger = logger;
            _messagePrefix = messagePrefix;
        }

        // This is just wrapping internal implementation so it can add a prefix
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var newState = default(object);

            if (state is FormattedLogValues logValues)
            {
                var dictionary = logValues.ToDictionary(e => e.Key, e => e.Value);
                var originalMessage = dictionary.Last().Value;
                dictionary.Remove(dictionary.Last().Key);

                var values = dictionary.Select(e => e.Value).ToArray();

                newState = new FormattedLogValues(
                    format: $"{_messagePrefix} {originalMessage}", values);
            }

            _logger.Log(logLevel, eventId, newState is null ? state : (TState)newState, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }
    }
}
