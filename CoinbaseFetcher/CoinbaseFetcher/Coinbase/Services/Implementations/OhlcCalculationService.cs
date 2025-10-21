using CoinbaseFetcher.Coinbase.Models;
using CoinbaseFetcher.Coinbase.Services.Interfaces;

namespace CoinbaseFetcher.Coinbase.Services.Implementations;

public class OhlcCalculationService : IOhlcCalculationService
{
    private readonly TimeSpan _interval;
    private DateTime _currentPeriodStart;
    private PeriodData _currentOHLC;
    private readonly object _lock = new object();

    public event Action<PeriodData> OnOhlcCalculated;

    public OhlcCalculationService(TimeSpan interval)
    {
        _interval = interval;
        InitializeNewPeriod(DateTime.UtcNow);
    }

    public void ProcessTick(WebSocketData ticker)
    {
        lock (_lock)
        {
            var tickTime = ticker.Time;
            
            if (tickTime >= _currentPeriodStart + _interval)
            {
                if (_currentOHLC != null)
                {
                    OnOhlcCalculated?.Invoke(_currentOHLC);
                }
                
                InitializeNewPeriod(tickTime);
            }
            
            UpdateOhlc(ticker.Price, tickTime);
        }
    }

    private void InitializeNewPeriod(DateTime tickTime)
    {
        _currentPeriodStart = new DateTime(
            tickTime.Year, tickTime.Month, tickTime.Day,
            tickTime.Hour, tickTime.Minute, 0, DateTimeKind.Utc
        );

        _currentOHLC = new PeriodData
        {
            Symbol = "BTC-USD",
            Timestamp = DateTime.UtcNow,
            PeriodStart = _currentPeriodStart,
            PeriodEnd = _currentPeriodStart + _interval
        };
    }

    private void UpdateOhlc(decimal price, DateTime time)
    {
        if (_currentOHLC.Open == 0)
        {
            _currentOHLC.Open = price;
            _currentOHLC.High = price;
            _currentOHLC.Low = price;
        }
        else
        {
            _currentOHLC.High = Math.Max(_currentOHLC.High, price);
            _currentOHLC.Low = Math.Min(_currentOHLC.Low, price);
        }
        
        _currentOHLC.Close = price;
        _currentOHLC.Timestamp = time;
    }
}