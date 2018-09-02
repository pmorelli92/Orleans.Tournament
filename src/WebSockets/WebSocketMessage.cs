namespace Snaelro.WebSockets
{
    public class WebSocketMessage
    {
        public string Type { get; set; }

        public object Payload { get; set; }

        public WebSocketMessage(string type, object payload)
        {
            Type = type;
            Payload = payload;
        }
    }
}