using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Tournament.Utils.Mvc.Middlewares;

namespace Orleans.Tournament.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            var logger = webHost.Services.GetService<ILogger<Program>>();
            var clusterClient = webHost.Services.GetService<IClusterClient>();
            var appStopper = webHost.Services.GetService<AppStopper>();

            // Connect to the Silo with retrial
            await clusterClient.Connect(e => RetryFilter(e, logger));

            // Starting API
            await webHost.RunAsync(appStopper.TokenSource.Token);

            // When appStopper blocker triggers, end connection to Silo
            await clusterClient.Close();
        }

        private static async Task<bool> RetryFilter(Exception exception, ILogger logger)
        {
            logger.LogWarning("Exception while connecting to Silo: {exception}", exception);
            await Task.Delay(TimeSpan.FromSeconds(2));
            return true;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
