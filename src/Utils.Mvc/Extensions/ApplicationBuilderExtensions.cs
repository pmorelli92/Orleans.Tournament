using Microsoft.AspNetCore.Builder;
using Orleans.Tournament.Utils.Mvc.Middlewares;

namespace Orleans.Tournament.Utils.Mvc.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVersionCheck(this IApplicationBuilder @this)
            => @this.Map("/version", b => b.UseMiddleware<VersionMiddleware>());

        public static IApplicationBuilder UseLeave(this IApplicationBuilder @this)
            => @this.Map("/leave", b => b.UseMiddleware<LeaveMiddleware>());
    }
}