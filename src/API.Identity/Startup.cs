using JWTSimpleServer.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Tournament.API.Identity.Authentication;
using Orleans.Tournament.Utils.Mvc.Configuration;
using Orleans.Tournament.Utils.Mvc.Extensions;

namespace Orleans.Tournament.API.Identity
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
                .AddSingleton<IUserStore>(e => new UserStore(e.GetService<FromEnvironment>().PostgresConnection))
                .AddJwtSimpleServer(setup => setup.IssuerSigningKey = _fromEnvironment.JwtIssuerKey)
                .AddJwtInMemoryRefreshTokenStore()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseJwtSimpleServer(setup => setup.IssuerSigningKey =  _fromEnvironment.JwtIssuerKey);
            appBuilder.UseVersionCheck();
            appBuilder.UseMvc();
        }
    }
}
