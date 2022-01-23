using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Tournament.API.Middlewares;
using Orleans.Tournament.Projections;
using Orleans.Tournament.Projections.Teams;
using Orleans.Tournament.Projections.Tournaments;
using Orleans.Tournament.Utils.Mvc.Configuration;
using Orleans.Tournament.Utils.Mvc.Extensions;
using Orleans.Tournament.Utils.Mvc.Middlewares;
using Orleans.CodeGeneration;
using Constants = Orleans.Tournament.Domain.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: KnownAssembly(typeof(Orleans.Tournament.Domain.Helpers))]

namespace Orleans.Tournament.API
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
                .AddSingleton(new PostgresOptions(_fromEnvironment.PostgresConnection))
                .AddJwtSimpleServer(setup => setup.IssuerSigningKey = _fromEnvironment.JwtIssuerKey);

            services
                .AddSingleton<ITeamQueryHandler, TeamQueryHandler>()
                .AddSingleton<ITournamentQueryHandler, TournamentQueryHandler>();

            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                });
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
                .AddSimpleMessageStreamProvider(Constants.MemoryProvider)
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
