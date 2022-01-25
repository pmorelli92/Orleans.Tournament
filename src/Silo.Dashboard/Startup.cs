using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Tournament.Utils.Mvc.Configuration;
using Orleans.Tournament.Utils.Mvc.Extensions;
using Orleans.Tournament.Utils.Mvc.Middlewares;

namespace Orleans.Tournament.Silo.Dashboard
{
    public class Startup
    {
        private readonly FromEnvironment _fromEnvironment;
        private static IClusterClient _clusterClient;

        public Startup(IConfiguration configuration)
        {
            _fromEnvironment = FromEnvironment.Build(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adds HTTP Logging and customises any other ILogger instances
            services.AddLogging(e => e.CustomJsonLogger());

            var clusterClient = CreateClient();

            services
                .AddSingleton(clusterClient)
                .AddSingleton((IGrainFactory)clusterClient)
                .AddSingleton(AppStopper.New)
                .AddSingleton(_fromEnvironment);

            services.AddServicesForSelfHostedDashboard(null, opt =>
            {
                opt.HideTrace = true;
                opt.Port = 80;
                opt.CounterUpdateIntervalMs = 5000;
            });
        }

        private IClusterClient CreateClient()
        {
            _clusterClient = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = _fromEnvironment.ClusterId;
                    options.ServiceId = _fromEnvironment.ServiceId;
                })
                .UseAdoNetClustering(opt =>
                {
                    opt.Invariant = _fromEnvironment.PostgresInvariant;
                    opt.ConnectionString = _fromEnvironment.PostgresConnection;
                })
                .ConfigureLogging(e => e.CustomJsonLogger())
                .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
                .UseDashboard()
                .Build();

            return _clusterClient;
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseLeave();
            appBuilder.UseVersionCheck();
            appBuilder.UseOrleansDashboard();
        }
    }
}
