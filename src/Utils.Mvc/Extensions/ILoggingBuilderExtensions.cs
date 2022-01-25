using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Orleans.Tournament.Utils.Mvc.Extensions
{
    public static class ILoggingBuilderExtensions 
    {
        public static ILoggingBuilder CustomJsonLogger(this ILoggingBuilder builder)
        {
            return builder
                .AddJsonConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.TimestampFormat = "dd/MM/yyyy hh:mm:ss";
                    options.JsonWriterOptions = new JsonWriterOptions
                    {
                        Indented = true
                    };
                })
                .AddFilter(level => level >= LogLevel.Information);
        }
    }
}
