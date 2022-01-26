using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.Tournament.Utils.Mvc.Middlewares;

namespace Orleans.Tournament.Silo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            var siloHost = webHost.Services.GetService<ISiloHost>();
            var appStopper = webHost.Services.GetService<AppStopper>();

            // Start the Silo
            await siloHost.StartAsync();

            // Starting API
            await webHost.RunAsync(appStopper.TokenSource.Token);

            // When appStopper blocker triggers, disconnect Silo from cluster
            await siloHost.StopAsync();
            await siloHost.Stopped;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}