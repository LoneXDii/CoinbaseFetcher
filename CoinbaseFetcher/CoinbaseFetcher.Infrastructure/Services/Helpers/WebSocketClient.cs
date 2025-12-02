using System.Net.WebSockets;

namespace CoinbaseFetcher.Infrastructure.Services.Helpers;

internal class WebSocketClient : IWebSocketClient
{
    private readonly ClientWebSocket _webSocket = new();

    public WebSocketState State => _webSocket.State;

    public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        return _webSocket.ConnectAsync(uri, cancellationToken);
    }

    public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
        return _webSocket.ReceiveAsync(buffer, cancellationToken);
    }

    public Task SendAsync(
        ArraySegment<byte> buffer,
        WebSocketMessageType messageType,
        bool endOfMessage,
        CancellationToken cancellationToken)
    {
        return _webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
    }
    
    public void Dispose()
    {
        _webSocket.Dispose();
    }
}
