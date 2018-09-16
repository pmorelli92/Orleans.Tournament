using JWTSimpleServer.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Snaelro.Utils.Mvc.Configuration;
using Snaelro.Utils.Mvc.Extensions;

namespace Snaelro.API.Identity
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
                .AddSingleton(_fromEnvironment)
                .AddSingleton<IAuthenticationProvider, AuthProvider>()
                .AddJwtSimpleServer(setup => setup.IssuerSigningKey = _fromEnvironment.JwtIssuerKey)
                .AddJwtInMemoryRefreshTokenStore()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseJwtSimpleServer(setup => setup.IssuerSigningKey =  _fromEnvironment.JwtIssuerKey);
            appBuilder.UseVersionCheck();
        }
    }
}