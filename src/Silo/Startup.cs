using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Utils.Mvc.Configuration;
using Snaelro.Utils.Mvc.Extensions;
using Snaelro.Utils.Mvc.Middlewares;

namespace Snaelro.Silo
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
            services.AddMvc();

            services.AddSingleton(CreateSilo());
            services.AddSingleton(AppStopper.New);
            services.AddSingleton(_fromEnvironment);
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
                    // opt.UseJsonFormat = true;
                })
                .AddLogStorageBasedLogConsistencyProviderAsDefault()
                .ConfigureEndpoints(_fromEnvironment.SiloPort, _fromEnvironment.GatewayPort)
                .ConfigureApplicationParts(parts => parts
                    .AddApplicationPart(typeof(Domain.Teams.Aggregates.TeamGrain).Assembly).WithReferences()
                    .AddApplicationPart(typeof(Projections.Teams.Subscriber).Assembly).WithReferences())
                .AddMemoryGrainStorage(_fromEnvironment.PubSubStore)
                .AddSimpleMessageStreamProvider(Constants.TeamStream)
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseLeave();
            appBuilder.UseVersionCheck();

            appBuilder.UseMvc();
        }
    }
}