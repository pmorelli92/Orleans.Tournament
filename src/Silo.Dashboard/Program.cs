using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Snaelro.Utils.Mvc.Middlewares;

namespace Snaelro.Silo.Dashboard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            var logger = webHost.Services.GetService<ILogger<Program>>();
            var clusterClient = webHost.Services.GetService<IClusterClient>();
            var appStopper = webHost.Services.GetService<AppStopper>();

            logger.LogInformation("[IClusterClient] connecting");
            await clusterClient.Connect(e => RetryFilter(e, logger));

            logger.LogInformation("[IWebHost] starting");
            await webHost.RunAsync(appStopper.TokenSource.Token);
            logger.LogInformation("[IWebHost] ending");

            logger.LogInformation("[IClusterClient] closing");
            await clusterClient.Close();
            logger.LogInformation("[IClusterClient] closed");
        }

        private static async Task<bool> RetryFilter(Exception exception, ILogger logger)
        {
            logger.LogWarning("[IClusterClient] exception while connecting: {exception}", exception.Demystify());
            await Task.Delay(TimeSpan.FromSeconds(2));
            return true;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}