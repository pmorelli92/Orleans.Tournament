using JWTSimpleServer.Abstractions;
using Orleans.Tournament.API.Identity;
using Orleans.Tournament.API.Identity.Authentication;
using Orleans.Tournament.Utils.Mvc.Configuration;
using Orleans.Tournament.Utils.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Parse environment variables
var env = FromEnvironment.Build(builder.Configuration);

builder
    .Services
    .AddLogging(e => e.CustomJsonLogger())
    .AddSingleton(env) // Required by the version middleware
    .AddSingleton<IAuthenticationProvider, AuthProvider>()
    .AddSingleton<IUserStore>(e => new UserStore(env.PostgresConnection))
    .AddJwtSimpleServer(setup => setup.IssuerSigningKey = env.JwtIssuerKey)
    .AddJwtInMemoryRefreshTokenStore()
    .AddControllers();

var app = builder.Build();

app.UseJwtSimpleServer(setup => setup.IssuerSigningKey = env.JwtIssuerKey);
app.UseVersionCheck();
app.UseRouting();
app.UseEndpoints(e => e.MapControllers());
app.Run();