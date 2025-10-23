using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace CoinbaseFetcher.Infrastructure.Services;

internal class WebSocketFetchingService : IWebSocketFetchingService
{
    private readonly ClientWebSocket _webSocketClient;
    private readonly WebSocketConfiguration _webSocketConfiguration;
    private readonly IMessageBus _messageBus;

    public WebSocketFetchingService(
        IOptions<WebSocketConfiguration> webSocketConfiguration,
        IMessageBus messageBus)
    {
        _webSocketClient = new ClientWebSocket();
        _webSocketConfiguration = webSocketConfiguration.Value;
        _messageBus = messageBus;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
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

        await ReceiveMessagesAsync(cancellationToken);
    }

    private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];

        while (!cancellationToken.IsCancellationRequested && _webSocketClient.State == WebSocketState.Open)
        {
            try
            {
                var result = await _webSocketClient.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    
                    var message = DeserializeTickData(messageJson);
                    
                    _messageBus.SendMessageReceivedEvent(message);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private TickData? DeserializeTickData(string json)
    {
        using var document = JsonDocument.Parse(json);
        var type = document.RootElement.GetProperty("type").GetString();
        
        if (type == "ticker")
        {
            return new TickData()
            {
                ProductId = document.RootElement.GetProperty("product_id").GetString(),
                Price =  decimal.Parse(document.RootElement.GetProperty("price").GetString(), CultureInfo.InvariantCulture),
                DateTime = document.RootElement.GetProperty("time").GetDateTime()
            };
        }

        return null;
    } 
    
    public void Dispose()
    {
        _webSocketClient.Dispose();
    }
}
