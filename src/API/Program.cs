using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Configuration;
using Orleans.Hosting;
using Tournament.API.Middlewares;
using Tournament.Domain;
using Tournament.Projections;
using Tournament.Projections.Teams;
using Tournament.Projections.Tournaments;
using Results = Microsoft.AspNetCore.Http.Results;

[assembly: KnownAssembly(typeof(Constants))]
var builder = WebApplication.CreateBuilder(args);

// Parse environment variables
IConfiguration configuration = builder.Configuration;
var clusterId = configuration["CLUSTER_ID"];
var serviceId = configuration["SERVICE_ID"];
var buildVersion = configuration["BUILD_VERSION"];
var postgresConnection = configuration["POSTGRES_CONNECTION"];
var jwtIssuer = configuration["JWT_ISSUER"];
var jwtAudience = configuration["JWT_AUDIENCE"];
var jwtSigningKey = Encoding.UTF8.GetBytes(configuration["JWT_SIGNING_KEY"]);

var postgresOptions = new PostgresOptions(postgresConnection);

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
    .AddSimpleMessageStreamProvider(Constants.InMemoryStream)
    .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
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
    .AddSingleton(clusterClient)
    .AddSingleton(postgresOptions)
    .AddSingleton<ITeamQueryHandler, TeamQueryHandler>()
    .AddSingleton<ITournamentQueryHandler, TournamentQueryHandler>()
    .AddControllers();

builder
    .Services
    .AddAuthorization()
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(jwtSigningKey),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true
    });

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/version", () => Results.Ok(new { BuildVersion = buildVersion }));
app.MapGet("/leave", () =>
{
    appStopper.Cancel();
    Results.Ok();
});

app.UseWebSockets();
app.Map("/ws", ws => ws.UseMiddleware<WebSocketPubSubMiddleware>());

app.UseEndpoints(e => e.MapControllers());

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