using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orleans.Tournament.API.Identity;

var builder = WebApplication.CreateBuilder(args);

// Parse environment variables
IConfiguration configuration = builder.Configuration;
var buildVersion = configuration["BUILD_VERSION"];
var postgresConnection = new ConnectionString(configuration["POSTGRES_CONNECTION"]);
var jwtIssuer = configuration["JWT_ISSUER"];
var jwtAudience = configuration["JWT_AUDIENCE"];
var jwtSigningKey = Encoding.UTF8.GetBytes(configuration["JWT_SIGNING_KEY"]);

var jwtConfiguration = new JwtConfiguration(jwtIssuer, jwtAudience, new SymmetricSecurityKey(jwtSigningKey));

builder
    .Services
    .AddLogging(e =>
        e.AddJsonConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "dd/MM/yyyy hh:mm:ss";
            options.JsonWriterOptions = new JsonWriterOptions
            {
                Indented = true
            };
        })
        .AddFilter(level => level >= LogLevel.Information)
    )
    .AddSingleton(jwtConfiguration)
    .AddSingleton(postgresConnection)
    .AddSingleton<ICreateUser, UserAuthentication>()
    .AddSingleton<ILoginUser, UserAuthentication>()
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
app.UseEndpoints(e => e.MapControllers());
app.Run();