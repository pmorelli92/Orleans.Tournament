using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Snaelro.Utils.Mvc.Middlewares;

namespace Snaelro.Silo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            var logger = webHost.Services.GetService<ILogger<Program>>();
            var siloHost = webHost.Services.GetService<ISiloHost>();
            var appStopper = webHost.Services.GetService<AppStopper>();

            logger.LogInformation("[ISiloHost] starting");
            await siloHost.StartAsync();

            logger.LogInformation("[IWebHost] starting");
            await webHost.RunAsync(appStopper.TokenSource.Token);
            logger.LogInformation("[IWebHost] ending");

            logger.LogInformation("[ISiloHost] stopping");
            await siloHost.StopAsync();

            await siloHost.Stopped;
            logger.LogInformation("[ISiloHost] stopped");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}