using System.Text.Json;
using CoinbaseFetcher.Coinbase.Models;
using CoinbaseFetcher.Coinbase.Services.Interfaces;
using CoinbaseFetcher.Email.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace CoinbaseFetcher.Coinbase.Services.Implementations;

public class CoinbaseDataProcessingService : BackgroundService
{
    private readonly IWebSocketService _webSocketService;
    private readonly IOhlcCalculationService _ohlcCalculator;
    private readonly IEmailService _emailService;

    public CoinbaseDataProcessingService(
        IWebSocketService webSocketService,
        IOhlcCalculationService ohlcCalculator,
        IEmailService emailService)
    {
        _webSocketService = webSocketService;
        _ohlcCalculator = ohlcCalculator;
        _emailService = emailService;
        
        _webSocketService.OnTickerReceived += ProcessTick;
        _ohlcCalculator.OnOhlcCalculated += SendEmail;
    }
    
    private void ProcessTick(WebSocketData ticker)
    {
        if (ticker.ProductId == "BTC-USD")
        {
            _ohlcCalculator.ProcessTick(ticker);
        }
    }
    
    private void SendEmail(PeriodData ohlc)
    {
        _emailService.SendEmail(ohlc);
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