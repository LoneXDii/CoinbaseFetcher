using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoinbaseFetcher.Coinbase.Models;
using CoinbaseFetcher.Coinbase.Services.Interfaces;
using CoinbaseFetcher.Configuration;
using Microsoft.Extensions.Options;

namespace CoinbaseFetcher.Coinbase.Services.Implementations;

public class WebSocketService : IWebSocketService
{
    private readonly ClientWebSocket _webSocketClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly WebSocketConfiguration _webSocketConfiguration;

    public event Action<WebSocketData> OnTickerReceived;

    public WebSocketService(IOptions<WebSocketConfiguration> webSocketConfiguration)
    {
        _webSocketClient = new ClientWebSocket();
        _cancellationTokenSource = new CancellationTokenSource();
        _webSocketConfiguration = webSocketConfiguration.Value;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
            await _webSocketClient.ConnectAsync(
                new Uri(_webSocketConfiguration.Url),
                cancellationToken);

            var subscribeMessage = new
            {
                type = "subscribe",
                product_ids = _webSocketConfiguration.Products,
                channels = _webSocketConfiguration.Channels,
            };

            var json = JsonSerializer.Serialize(subscribeMessage);
            var buffer = Encoding.UTF8.GetBytes(json);
            
            await _webSocketClient.SendAsync(
                new ArraySegment<byte>(buffer), 
                WebSocketMessageType.Text,
                true,
                cancellationToken);

            await Task.Run(StartReceiving, cancellationToken);
    }

    private async Task StartReceiving()
    {
        var buffer = new byte[4096];
        
        while (_webSocketClient.State == WebSocketState.Open && !_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var result = await _webSocketClient.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    _cancellationTokenSource.Token);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ProcessMessage(message);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private void ProcessMessage(string message)
    {
        using var document = JsonDocument.Parse(message);
        var type = document.RootElement.GetProperty("type").GetString();

        var jsonOptions = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        
        if (type == "ticker")
        {
            var tickData = JsonSerializer.Deserialize<WebSocketData>(message, jsonOptions);

            OnTickerReceived?.Invoke(tickData);
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _webSocketClient?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}