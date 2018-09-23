using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Snaelro.Tests.WebSockets.Client
{
    public class Program
    {
        private static Uri _uri;
        private static string _accessToken;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Insert token: ");
            _accessToken = Console.ReadLine();

            Console.WriteLine("Choose environment (M)inikube / (L)ocalhost: ");
            var env = Console.ReadLine();

            if (string.IsNullOrEmpty(env))
                return;

            if (env.ToUpperInvariant() == "M")
                _uri = new Uri($"ws://192.168.99.100:30701/ws");
            else if (env.ToUpperInvariant() == "L")
                _uri = new Uri($"ws://localhost:7001/ws");
            else
                return;

            Console.WriteLine("Starting -> Press q to quit");
            var client = new ClientWebSocket();
            client.Options.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
            var cts = new CancellationTokenSource();

            await Task.Factory.StartNew(async () =>
                {
                    await client.ConnectAsync(_uri, CancellationToken.None);

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

                            var formatted = JToken.Parse(message).ToString(Formatting.Indented);

                            Console.WriteLine(formatted);
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
        }
    }
}
