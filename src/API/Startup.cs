using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Snaelro.API.Middlewares;
using Snaelro.Utils.Mvc.Configuration;
using Snaelro.Utils.Mvc.Extensions;
using Snaelro.Utils.Mvc.Middlewares;

namespace Snaelro.API
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
                .AddSingleton(CreateClient())
                .AddSingleton(AppStopper.New)
                .AddSingleton(_fromEnvironment)
                .AddJwtSimpleServer(setup => setup.IssuerSigningKey = _fromEnvironment.JwtIssuerKey)
                .AddMvc();
        }

        private IClusterClient CreateClient()
        {
            return new ClientBuilder()
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
                .AddSimpleMessageStreamProvider("ws")
                .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
                .Build();
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseLeave();
            appBuilder.UseVersionCheck();
            appBuilder.UseJwtSimpleServer(setup => setup.IssuerSigningKey = _fromEnvironment.JwtIssuerKey);

            appBuilder.UseWebSockets();
            appBuilder.Map("/ws", ws => ws.UseMiddleware<WebSocketPubSubMiddleware>());
            appBuilder.UseMvc();
        }
    }
}