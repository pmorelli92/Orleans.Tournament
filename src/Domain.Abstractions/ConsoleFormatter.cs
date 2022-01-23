using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Orleans.Tournament.Domain.Abstractions
{
    public class PrefixFormatter : ConsoleFormatter
    {
        private readonly string _messagePrefix;

        public PrefixFormatter(string messagePrefix)
            :base(messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (message == null)
                return;

            textWriter.Write($"{_messagePrefix} ");
            textWriter.Write(message);

        }
    }
}
