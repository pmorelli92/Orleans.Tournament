using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Tournament.API.Middlewares;
using Orleans.Tournament.Domain;
using Orleans.Tournament.Projections;
using Orleans.Tournament.Projections.Teams;
using Orleans.Tournament.Projections.Tournaments;
using Orleans.Tournament.Utils.Mvc.Extensions;
using Orleans.Tournament.Utils.Mvc.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Parse environment variables
IConfiguration configuration = builder.Configuration;
var buildVersion = configuration["BUILD_VERSION"];
var postgresConnection = new PostgresOptions(configuration["POSTGRES_CONNECTION"]);
var jwtConfiguration = new JwtConfiguration(
    configuration["JWT_ISSUER"],
    configuration["JWT_AUDIENCE"],
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_SIGNING_KEY"])));

var clusterId = configuration["CLUSTER_ID"];
var serviceId = configuration["SERVICE_ID"];

builder
    .Services
    .AddLogging(e =>
        e.AddJsonConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "dd/MM/yyyy hh:mm:ss";
            options.JsonWriterOptions = new JsonWriterOptions { Indented = true };
        })
        .AddFilter(level => level >= LogLevel.Information)
    )
    .AddSingleton(jwtConfiguration)
    .AddSingleton(postgresConnection)
    .AddSingleton<ITeamQueryHandler, TeamQueryHandler>()
    .AddSingleton<ITournamentQueryHandler, TournamentQueryHandler>()
    .AddSingleton(new ClientBuilder()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = clusterId;
            options.ServiceId = serviceId;
        })
        .UseAdoNetClustering(opt =>
        {
            opt.Invariant = "Npgsql";
            opt.ConnectionString = postgresConnection.ConnectionString;
        })
        .ConfigureLogging(e =>
            e.AddJsonConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.TimestampFormat = "dd/MM/yyyy hh:mm:ss";
                    options.JsonWriterOptions = new JsonWriterOptions { Indented = true };
                })
                .AddFilter(level => level >= LogLevel.Information)
        )
        .AddSimpleMessageStreamProvider(Helpers.MemoryProvider)
        .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
        .Build())
    .AddSingleton(AppStopper.New)
    .AddControllers();

builder
    .Services
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
        ValidIssuer = jwtConfiguration.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtConfiguration.Audience,
        IssuerSigningKey = jwtConfiguration.SigningKey,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true
    });

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/version", () => Results.Ok(new {BuildVersion = buildVersion}));

app.UseLeave();
app.UseWebSockets();
app.Map("/ws", ws => ws.UseMiddleware<WebSocketPubSubMiddleware>());

app.UseEndpoints(e => e.MapControllers());

// Connect to cluster
var clusterClient = app.Services.GetService<IClusterClient>();
var appStopper = app.Services.GetService<AppStopper>();

await clusterClient.Connect(async e =>
{
    await Task.Delay(TimeSpan.FromSeconds(2));
    return true;
});

// Starting API
await app.RunAsync(appStopper.TokenSource.Token);

// When /leave is invoked, the appStopper will be cancelled and this code gets executed

await clusterClient.Close();

public record JwtConfiguration(string Issuer, string Audience, SymmetricSecurityKey SigningKey);