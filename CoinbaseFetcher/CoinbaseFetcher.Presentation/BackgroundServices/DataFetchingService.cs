using CoinbaseFetcher.Domain.Interfaces;
using Microsoft.Extensions.Hosting;

namespace CoinbaseFetcher.Presentation.BackgroundServices;

public class DataFetchingService : BackgroundService
{
    private readonly IWebSocketFetchingService _webSocketFetchingService;

    public DataFetchingService(IWebSocketFetchingService webSocketFetchingService)
    {
        _webSocketFetchingService = webSocketFetchingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _webSocketFetchingService.ConnectAsync(stoppingToken);
    }
    
    public override void Dispose()
    {
        _webSocketFetchingService.Dispose();
        base.Dispose();
    }
}
