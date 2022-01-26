using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Orleans.Tournament.Utils.Mvc.Middlewares
{
    public class LeaveMiddleware
    {
        private readonly AppStopper _appStopper;
        private readonly RequestDelegate _next;

        public LeaveMiddleware(
            AppStopper appStopper,
            RequestDelegate next)
        {
            _next = next;
            _appStopper = appStopper;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            _appStopper.TokenSource.Cancel();
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("stopping application...");
        }
    }

    public class AppStopper
    {
        public CancellationTokenSource TokenSource { get; }

        private AppStopper(CancellationTokenSource tokenSource)
        {
            TokenSource = tokenSource;
        }

        public static AppStopper New => new AppStopper(new CancellationTokenSource());
    }
}