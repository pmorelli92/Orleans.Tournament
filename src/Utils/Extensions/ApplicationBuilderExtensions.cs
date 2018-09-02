using Microsoft.AspNetCore.Builder;
using Snaelro.Utils.Middlewares;

namespace Snaelro.Utils.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVersionCheck(this IApplicationBuilder @this)
            => @this.Map("/version", b => b.UseMiddleware<VersionMiddleware>());
    }
}