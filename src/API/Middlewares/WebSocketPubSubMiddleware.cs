using System.Net;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Orleans;
using Orleans.Streams;
using Tournament.Domain;
using Tournament.WebSockets;

namespace Tournament.API.Middlewares;

public class WebSocketPubSubMiddleware
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<WebSocketPubSubMiddleware> _logger;

    public WebSocketPubSubMiddleware(
        RequestDelegate _,
        IClusterClient clusterClient,
        ILogger<WebSocketPubSubMiddleware> logger)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    public async Task Invoke(HttpContext context)
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

        var subscription = default(StreamSubscriptionHandle<WebSocketMessage>);
        var userId = new Guid(auth.Ticket.Principal.Claims.Single(e => e.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            _logger.LogInformation("[Websocket] opened connection for UserId: {userId}", userId);

            subscription = await
                _clusterClient
                .GetStreamProvider(Constants.InMemoryStream)
                .GetStream<WebSocketMessage>(userId, Constants.WebSocketNamespace)
                .SubscribeAsync(async (evt, st) =>
                {
                    var bytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(evt));
                    var msgBuffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await webSocket.SendAsync(msgBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                });

            var buffer = new byte[1024 * 4];

            while (webSocket.CloseStatus.HasValue == false)
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            await webSocket.CloseAsync(
                webSocket.CloseStatus.Value,
                webSocket.CloseStatusDescription, CancellationToken.None);

            _logger.LogInformation("[Websocket] closed connection for TraceId: {traceId}", userId);
        }
        catch (Exception e)
        {
            _logger.LogError("[Websocket] disconnect error -> {exception}", e);
        }
        finally
        {
            if (subscription != null)
                await subscription.UnsubscribeAsync();
        }
    }
}