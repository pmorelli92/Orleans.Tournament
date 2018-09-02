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

            logger.LogInformation("Silo -> Starting");
            await Startup.SiloHost.StartAsync();

            await webHost.RunAsync(Startup.StopExecution.Token);
            logger.LogInformation("Web Host -> Ending");

            logger.LogInformation("Silo -> Stopping");

            await Startup.SiloHost.StopAsync();
            await Startup.SiloHost.Stopped;

            logger.LogInformation("Silo -> Stopped");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}