using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Streams;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Utils.Mvc.Configuration;
using Snaelro.Utils.Mvc.Extensions;
using Snaelro.Utils.Mvc.Middlewares;

namespace Snaelro.API
{
    public class Startup
    {
        private readonly FromEnvironment _fromEnvironment;
        private IClusterClient _clusterClient;

        public Startup(IConfiguration configuration)
        {
            _fromEnvironment = FromEnvironment.Build(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(CreateClient())
                .AddSingleton(AppStopper.New)
                .AddSingleton(_fromEnvironment)
                .AddJwtSimpleServer(setup => setup.IssuerSigningKey = _fromEnvironment.JwtIssuerKey)
                .AddMvc();
        }

        private IClusterClient CreateClient()
        {
            _clusterClient = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = _fromEnvironment.ClusterId;
                    options.ServiceId = _fromEnvironment.ServiceId;
                })
                .UseAdoNetClustering(opt =>
                {
                    opt.Invariant = _fromEnvironment.PostgresInvariant;
                    opt.ConnectionString = _fromEnvironment.PostgresConnection;
                })
                .AddSimpleMessageStreamProvider("ws")
                .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
                .Build();

            return _clusterClient;
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseLeave();
            appBuilder.UseVersionCheck();

            appBuilder.UseWebSockets();
            appBuilder.Map("/ws", ws =>
            {
                ws.Use(async (context, next) =>
                {
                    if (!context.WebSockets.IsWebSocketRequest)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return;
                    }

                    var auth = await context.AuthenticateAsync();

                    if (!auth.Succeeded)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }

                    var subscription = default(StreamSubscriptionHandle<object>);
                    var logger = appBuilder.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                    var userId = new Guid(auth.Ticket.Principal.Claims.Single(e => e.Type == ClaimTypes.NameIdentifier).Value);

                    try
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        logger.LogInformation("[Websocket] opened connection for UserId: {userId}", userId);

                        var streamProvider = _clusterClient.GetStreamProvider(name: "ws");
                        subscription =
                            await streamProvider
                                .GetStream<object>(userId, Constants.StreamNamespace)
                                .SubscribeAsync(async (evt, st) =>
                                {
                                    var serialized = JsonConvert.SerializeObject(evt);
                                    var bytes = Encoding.UTF8.GetBytes(serialized);
                                    var msgBuffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
                                    await webSocket.SendAsync(msgBuffer, WebSocketMessageType.Text, true,
                                        CancellationToken.None);
                                });

                        var buffer = new byte[1024 * 4];
                        var connected = true;

                        while (connected)
                        {
                            var res = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                                CancellationToken.None);
                            connected = res.CloseStatus.HasValue == false;
                        }

                        await webSocket.CloseAsync(
                            webSocket.CloseStatus.Value,
                            webSocket.CloseStatusDescription, CancellationToken.None);

                        logger.LogInformation("[Websocket] closed connection for TraceId: {traceId}", userId);
                    }
                    catch (Exception e)
                    {
                        logger.LogError("[Websocket] disconnect error -> {exception}", e.Demystify());
                    }
                    finally
                    {
                        if (subscription != null)
                            await subscription.UnsubscribeAsync();
                    }
                });
            });

            appBuilder.UseJwtSimpleServer(setup => setup.IssuerSigningKey = _fromEnvironment.JwtIssuerKey);
            appBuilder.UseMvc();
        }
    }
}