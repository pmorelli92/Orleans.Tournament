using System.Text.Json;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Statistics;
using Orleans.Tournament.Domain;
using Orleans.Tournament.Projections;
using Orleans.Tournament.Projections.Teams;
using Orleans.Tournament.Projections.Tournaments;

[assembly: KnownAssembly(typeof(Helpers))]
var builder = WebApplication.CreateBuilder(args);

// Parse environment variables
IConfiguration configuration = builder.Configuration;

var siloPort = configuration.GetValue<int>("SILO_PORT");
var gatewayPort = configuration.GetValue<int>("GATEWAY_PORT");

var clusterId = configuration["CLUSTER_ID"];
var serviceId = configuration["SERVICE_ID"];
var buildVersion = configuration["BUILD_VERSION"];
var postgresConnection = configuration["POSTGRES_CONNECTION"];

var postgresOptions = new PostgresOptions(postgresConnection);

var appStopper = new CancellationTokenSource();

// Orleans cluster silo
var clusterSilo = new SiloHostBuilder()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = clusterId;
        options.ServiceId = serviceId;
    })
    .UseAdoNetClustering(opt =>
    {
        opt.Invariant = "Npgsql";
        opt.ConnectionString = postgresConnection;
    })
    .AddAdoNetGrainStorageAsDefault(opt =>
    {
        opt.Invariant = "Npgsql";
        opt.ConnectionString = postgresConnection;
        //opt.UseJsonFormat = true; // TODO: After restart this does not apply the events
    })
    .AddLogStorageBasedLogConsistencyProviderAsDefault()
    .ConfigureEndpoints(siloPort, gatewayPort)
    .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
    .AddMemoryGrainStorage("PubSubStore")
    .AddSimpleMessageStreamProvider(Helpers.MemoryProvider)
    .ConfigureLogging(e =>
        e.AddJsonConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "dd/MM/yyyy hh:mm:ss";
            options.JsonWriterOptions = new JsonWriterOptions { Indented = true };
        })
        .AddFilter(level => level >= LogLevel.Warning)
    )
    // These dependencies are resolved by Orleans runtime so they are
    // configured inside the cluster creation
    .ConfigureServices(services =>
    {
        services
            .AddSingleton(postgresOptions)
            .AddSingleton<ITeamQueryHandler, TeamQueryHandler>()
            .AddSingleton<ITournamentQueryHandler, TournamentQueryHandler>();
    })
    .UseLinuxEnvironmentStatistics()
    .UseDashboard(options =>
    {
        options.HostSelf = false;
        options.CounterUpdateIntervalMs = 5000;
    })
    .Build();

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
    .AddSingleton(clusterSilo);

var app = builder.Build();

app.MapGet("/version", () => Results.Ok(new { BuildVersion = buildVersion }));
app.MapGet("/leave", () =>
{
    appStopper.Cancel();
    Results.Ok();
});

// Start the Silo
await clusterSilo.StartAsync();

// Starting API
await app.RunAsync(appStopper.Token);

// When /leave is invoked, the appStopper will be cancelled and this code gets executed
await clusterSilo.StopAsync();
await clusterSilo.Stopped;