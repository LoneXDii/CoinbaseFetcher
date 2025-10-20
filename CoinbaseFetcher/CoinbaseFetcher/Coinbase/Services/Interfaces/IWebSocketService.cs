using CoinbaseFetcher.Coinbase.Models;

namespace CoinbaseFetcher.Coinbase.Services.Interfaces;

public interface IWebSocketService : IDisposable
{
    event Action<WebSocketData> OnTickerReceived;
    
    Task ConnectAsync(CancellationToken cancellationToken = default);
}