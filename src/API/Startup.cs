using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
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
            services.AddMvc();

            services.AddSingleton(CreateClient());
            services.AddSingleton(AppStopper.New);
            services.AddSingleton(_fromEnvironment);
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
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        //TODO: Manage authentication

                        var logger = appBuilder.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                        var subscription = default(StreamSubscriptionHandle<object>);

                        try
                        {
                            //TODO: Get the TraceId that the user wants to hear
                            Guid.TryParse(
                                context.Request.Query.FirstOrDefault(e => e.Key == "traceId").Value,
                                out var traceId);

                            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            logger.LogInformation("[Websocket] opened connection for TraceId: {traceId}", traceId);

                            var streamProvider = _clusterClient.GetStreamProvider(name: "ws");
                            subscription =
                                await streamProvider
                                    .GetStream<object>(traceId, Constants.StreamNamespace)
                                    .SubscribeAsync(async (evt, st) =>
                                    {
                                        var serialized = JsonConvert.SerializeObject(evt);
                                        var bytes = Encoding.UTF8.GetBytes(serialized);
                                        var msgBuffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
                                        await webSocket.SendAsync(msgBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                                    });

                            var buffer = new byte[1024 * 4];
                            var connected = true;

                            while (connected)
                            {
                                var res = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                                connected = res.CloseStatus.HasValue == false;
                            }

                            await webSocket.CloseAsync(
                                webSocket.CloseStatus.Value,
                                webSocket.CloseStatusDescription, CancellationToken.None);

                            logger.LogInformation("[Websocket] closed connection for TraceId: {traceId}", traceId);

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
                    }
                    else
                    {
                        await next();
                    }
                });
            });


            appBuilder.UseMvc();
        }
    }
}