namespace Tournament.WebSockets;

public record WebSocketMessage(string Type, object Payload);