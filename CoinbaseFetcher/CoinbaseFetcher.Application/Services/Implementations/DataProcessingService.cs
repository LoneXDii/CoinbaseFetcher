using CoinbaseFetcher.Application.Configuration;
using CoinbaseFetcher.Application.Services.Interfaces;
using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;
using Microsoft.Extensions.Options;

namespace CoinbaseFetcher.Application.Services.Implementations;

internal class DataProcessingService : IDataProcessingService
{
    private readonly IMessageBus _messageBus;
    private readonly TimeSpan _calculationPeriodInterval;
    
    private DateTime _currentPeriodStart;
    private PeriodData? _currentPeriodData;

    public DataProcessingService(
        IMessageBus messageBus,
        IOptions<CalculationPeriodConfiguration> calculationPeriodConfiguration)
    {
        _messageBus = messageBus;
        _calculationPeriodInterval = TimeSpan.FromMinutes(calculationPeriodConfiguration.Value.CalculationIntervalInMinutes);
    }

    public void StartDataProcessing()
    {
        InitializeNewPeriod(DateTime.UtcNow);
        _messageBus.OnMessageReceived += ProcessTick;
    }

    private void ProcessTick(TickData tickData)
    {
        var tickTime = tickData.DateTime;
        
        if (tickTime >= _currentPeriodStart + _calculationPeriodInterval)
        {
            if (_currentPeriodData != null)
            {
                _messageBus.SendPeriodDataCalculatedEvent(_currentPeriodData);
            }
            
            InitializeNewPeriod(tickTime);
        }
        
        UpdateCurrentPeriodData(tickData.Price, tickTime);
    }
    
    private void InitializeNewPeriod(DateTime tickTime)
    {
        _currentPeriodStart = new DateTime(
            tickTime.Year, tickTime.Month, tickTime.Day,
            tickTime.Hour, tickTime.Minute, 0, DateTimeKind.Utc
        );

        _currentPeriodData = new PeriodData
        {
            Symbol = "BTC-USD",
            Timestamp = DateTime.UtcNow,
            PeriodStart = _currentPeriodStart,
            PeriodEnd = _currentPeriodStart + _calculationPeriodInterval
        };
    }
    
    private void UpdateCurrentPeriodData(decimal price, DateTime time)
    {
        if (_currentPeriodData.Open == 0)
        {
            _currentPeriodData.Open = price;
            _currentPeriodData.High = price;
            _currentPeriodData.Low = price;
        }
        else
        {
            _currentPeriodData.High = Math.Max(_currentPeriodData.High, price);
            _currentPeriodData.Low = Math.Min(_currentPeriodData.Low, price);
        }
        
        _currentPeriodData.Close = price;
        _currentPeriodData.Timestamp = time;
    }
}
