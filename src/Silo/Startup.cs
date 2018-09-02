using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Snaelro.Domain.Aggregates;
using Snaelro.Silo.Middlewares;
using Snaelro.Utils.Configuration;
using Snaelro.Utils.Extensions;

namespace Snaelro.Silo
{
    public class Startup
    {
        internal static ISiloHost SiloHost;
        internal static CancellationTokenSource StopExecution;

        private readonly FromEnvironment _fromEnvironment;

        public Startup(IConfiguration configuration)
        {
            _fromEnvironment = FromEnvironment.Build(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(_fromEnvironment);

            SiloHost = CreateSilo();
            StopExecution = new CancellationTokenSource();
        }

        private ISiloHost CreateSilo()
        {
            SiloHost = new SiloHostBuilder()
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
                })
                .AddStateStorageBasedLogConsistencyProviderAsDefault()
                .ConfigureEndpoints(_fromEnvironment.SiloPort, _fromEnvironment.GatewayPort)
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(TeamGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            return SiloHost;
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseVersionCheck();
            appBuilder.Map("/leave", b => b.UseMiddleware<LeaveMiddleware>());
            appBuilder.UseMvc();
        }
    }
}