using System.Net.WebSockets;
using System.Text;

Console.WriteLine("Insert token: ");
var accessToken = Console.ReadLine();

Console.WriteLine("Choose environment (K)ind / (L)ocalhost: ");
var env = Console.ReadLine();

if (string.IsNullOrEmpty(env))
    return;

var uri = new Uri("ws://localhost:7003/ws");

if (env.ToUpperInvariant() == "K")
    uri = new Uri("ws://localhost:30703/ws");
else
    return;

Console.WriteLine("Starting -> Press q to quit");
var client = new ClientWebSocket();
client.Options.SetRequestHeader("Authorization", $"Bearer {accessToken}");
var cts = new CancellationTokenSource();

await Task.Factory.StartNew(async () =>
{
    await client.ConnectAsync(uri, CancellationToken.None);

    if (client.State != WebSocketState.Open)
    {
        Console.WriteLine("Could not connect");
        return;
    }

    Console.WriteLine("Connected!");

    while (true)
    {
        var buffer = new byte[1024 * 4];
        var segment = new ArraySegment<byte>(buffer);
        var result = await client.ReceiveAsync(segment, CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
            break;

        if (result.MessageType == WebSocketMessageType.Text)
        {
            var count = result.Count;
            while (!result.EndOfMessage)
            {
                if (count >= buffer.Length)
                {
                    await client.CloseAsync(
                        WebSocketCloseStatus.InvalidPayloadData,
                        "That's too long",
                        CancellationToken.None);

                    break;
                }

                segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                result = await client.ReceiveAsync(segment, CancellationToken.None);
                count += result.Count;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, count);
            Console.WriteLine(message);
            Console.WriteLine("\n-------\n");
        }
    }
},
cts.Token,
TaskCreationOptions.LongRunning,
TaskScheduler.Default);

while (Console.ReadKey(true).Key != ConsoleKey.Q)
{
}

Console.WriteLine("Ending!");

cts.Cancel();
await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);