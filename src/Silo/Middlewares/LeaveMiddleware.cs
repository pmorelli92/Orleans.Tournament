using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Snaelro.Silo.Middlewares
{
    public class LeaveMiddleware
    {
        private readonly RequestDelegate _next;

        public LeaveMiddleware(RequestDelegate next)
            => _next = next;

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            Startup.StopExecution.Cancel();
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("Stopping silo");
        }
    }
}