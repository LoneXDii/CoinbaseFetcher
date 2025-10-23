namespace CoinbaseFetcher.Domain.Interfaces;

public interface IWebSocketFetchingService : IDisposable
{
    Task ConnectAsync(CancellationToken cancellationToken);
}
