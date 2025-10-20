using System.Text.Json;
using CoinbaseFetcher.Coinbase.Models;
using CoinbaseFetcher.Coinbase.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace CoinbaseFetcher.Coinbase.Services.Implementations;

public class CoinbaseDataProcessingService : BackgroundService
{
    private readonly IWebSocketService _webSocketService;
    private readonly IOhlcCalculationService _ohlcCalculator;

    public CoinbaseDataProcessingService(
        IWebSocketService webSocketService,
        IOhlcCalculationService ohlcCalculator)
    {
        _webSocketService = webSocketService;
        _ohlcCalculator = ohlcCalculator;
        
        _webSocketService.OnTickerReceived += OnTickerReceived;
        _ohlcCalculator.OnOhlcCompleted += OnOhlcCalculated;
    }
    
    private void OnTickerReceived(WebSocketData ticker)
    {
        if (ticker.ProductId == "BTC-USD")
        {
            _ohlcCalculator.ProcessTick(ticker);
        }
    }
    
    private void OnOhlcCalculated(PeriodData ohlc)
    {
        Console.WriteLine($"Period data: {JsonSerializer.Serialize(ohlc)}");
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _webSocketService.ConnectAsync(stoppingToken);
    }
    
    public override void Dispose()
    {
        _webSocketService.Dispose();
        base.Dispose();
    }
}