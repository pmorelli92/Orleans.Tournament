using System;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Snaelro.Silo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            var logger = webHost.Services.GetService<ILogger<Program>>();

            logger.LogInformation("Starting silo");
            await Startup.SiloHost.StartAsync();

            await webHost.RunAsync(Startup.StopExecution.Token);
            logger.LogInformation("Stopping web host");

            await Startup.SiloHost.StopAsync();
            logger.LogInformation("Stopping silo");

            await Startup.SiloHost.Stopped;
            logger.LogInformation("Waiting for silo to stop");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}