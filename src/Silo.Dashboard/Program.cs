using System.Text.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Parse environment variables
IConfiguration configuration = builder.Configuration;
var clusterId = configuration["CLUSTER_ID"];
var serviceId = configuration["SERVICE_ID"];
var buildVersion = configuration["BUILD_VERSION"];
var postgresConnection = configuration["POSTGRES_CONNECTION"];

var appStopper = new CancellationTokenSource();

// Orleans cluster connection
var clusterClient = new ClientBuilder()
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
    .ConfigureLogging(e =>
            e.AddJsonConsole(options =>
            {
                options.IncludeScopes = true;
                options.TimestampFormat = "dd/MM/yyyy hh:mm:ss";
                options.JsonWriterOptions = new JsonWriterOptions { Indented = true };
            })
            .AddFilter(level => level >= LogLevel.Warning)
    )
    .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
    .UseDashboard()
    .Build();

builder
    .Services
    .AddSingleton(clusterClient)
    .AddSingleton((IGrainFactory)clusterClient)
    .AddServicesForSelfHostedDashboard(null, opt =>
    {
        opt.HideTrace = true;
        opt.Port = 80;
        opt.CounterUpdateIntervalMs = 5000;
    });

var app = builder.Build();

app.UseOrleansDashboard();
app.MapGet("/version", () => Results.Ok(new { BuildVersion = buildVersion }));
app.MapGet("/leave", () =>
{
    appStopper.Cancel();
    Results.Ok();
});

// Connect to cluster
await clusterClient.Connect(async e =>
{
    await Task.Delay(TimeSpan.FromSeconds(2));
    return true;
});

// Starting API
await app.RunAsync(appStopper.Token);

// When /leave is invoked, the appStopper will be cancelled and this code gets executed
await clusterClient.Close();