using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Snaelro.Utils.Mvc.Configuration;

namespace Snaelro.Utils.Mvc.Middlewares
{
    public class VersionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FromEnvironment _fromEnvironment;

        public VersionMiddleware(
            RequestDelegate next,
            FromEnvironment fromEnvironment)
        {
            _next = next;
            _fromEnvironment = fromEnvironment;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            var buildVersion = _fromEnvironment.BuildVersion;
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(buildVersion);
        }
    }
}
