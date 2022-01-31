using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orleans.CodeGeneration;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Tournament.API.Middlewares;
using Orleans.Tournament.Projections;
using Orleans.Tournament.Projections.Teams;
using Orleans.Tournament.Projections.Tournaments;
using Orleans.Tournament.Utils.Mvc.Configuration;
using Orleans.Tournament.Utils.Mvc.Extensions;
using Orleans.Tournament.Utils.Mvc.Middlewares;
using Constants = Orleans.Tournament.Domain.Helpers;

[assembly: KnownAssembly(typeof(Constants))]

namespace Orleans.Tournament.API
{
    public record JwtConfiguration(string Issuer, string Audience, SymmetricSecurityKey SigningKey);
    
    public class Startup
    {
        private readonly FromEnvironment _fromEnvironment;
        private readonly JwtConfiguration _jwtConfiguration;

        public Startup(IConfiguration configuration)
        {
            _fromEnvironment = FromEnvironment.Build(configuration);
            _jwtConfiguration = new JwtConfiguration(
                configuration["JWT_ISSUER"],
                configuration["JWT_AUDIENCE"],
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_SIGNING_KEY"])));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adds HTTP Logging and customises any other ILogger instances
            services.AddLogging(e => e.CustomJsonLogger());

            services
                .AddSingleton(CreateClient())
                .AddSingleton(AppStopper.New)
                .AddSingleton(_fromEnvironment)
                .AddSingleton(new PostgresOptions(_fromEnvironment.PostgresConnection));

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
            
            
            services
                .AddAuthorization()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtConfiguration.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtConfiguration.Audience,
                    IssuerSigningKey = _jwtConfiguration.SigningKey,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
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
                .ConfigureLogging(e => e.CustomJsonLogger())
                .AddSimpleMessageStreamProvider(Constants.MemoryProvider)
                .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
                .Build();
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseLeave();
            appBuilder.UseVersionCheck();
            
            appBuilder.UseAuthentication();
            appBuilder.UseAuthorization();
            
            appBuilder.UseWebSockets();
            appBuilder.Map("/ws", ws => ws.UseMiddleware<WebSocketPubSubMiddleware>());
            appBuilder.UseMvc();
        }
    }
}