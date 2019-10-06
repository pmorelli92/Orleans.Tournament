using Orleans.CodeGeneration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Statistics;
using Orleans.Tournament.Projections;
using Orleans.Tournament.Projections.Teams;
using Orleans.Tournament.Projections.Tournaments;
using Orleans.Tournament.Utils.Mvc.Configuration;
using Orleans.Tournament.Utils.Mvc.Extensions;
using Orleans.Tournament.Utils.Mvc.Middlewares;
using Constants = Orleans.Tournament.Domain.Helpers;
[assembly: KnownAssembly(typeof(Orleans.Tournament.Domain.Helpers))]

namespace Orleans.Tournament.Silo
{
    public class Startup
    {
        private readonly FromEnvironment _fromEnvironment;

        public Startup(IConfiguration configuration)
        {
            _fromEnvironment = FromEnvironment.Build(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(CreateSilo())
                .AddSingleton(AppStopper.New)
                .AddSingleton(_fromEnvironment);
        }

        public void OrleansDependencyInjection(IServiceCollection services)
        {
            services
                .AddSingleton(new PostgresOptions(_fromEnvironment.PostgresConnection))
                .AddSingleton<ITeamQueryHandler, TeamQueryHandler>()
                .AddSingleton<ITournamentQueryHandler, TournamentQueryHandler>();
        }

        private ISiloHost CreateSilo()
        {
            return new SiloHostBuilder()
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
                .AddAdoNetGrainStorageAsDefault(opt =>
                {
                    opt.Invariant = _fromEnvironment.PostgresInvariant;
                    opt.ConnectionString = _fromEnvironment.PostgresConnection;
                    //opt.UseJsonFormat = true; // TODO: After restart this does not apply the events
                })
                .AddLogStorageBasedLogConsistencyProviderAsDefault()
                .ConfigureEndpoints(_fromEnvironment.SiloPort, _fromEnvironment.GatewayPort)
                .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
                .AddMemoryGrainStorage(_fromEnvironment.PubSubStore)
                .AddSimpleMessageStreamProvider("ws")
                .AddSimpleMessageStreamProvider(Constants.TeamStream)
                .AddSimpleMessageStreamProvider(Constants.TournamentStream)
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureServices(OrleansDependencyInjection)
                //.UseLinuxEnvironmentStatistics()
                .UseDashboard(options =>
                {
                    options.HostSelf = false;
                    options.CounterUpdateIntervalMs = 5000;
                })
                .Build();
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseLeave();
            appBuilder.UseVersionCheck();
        }
    }
}
