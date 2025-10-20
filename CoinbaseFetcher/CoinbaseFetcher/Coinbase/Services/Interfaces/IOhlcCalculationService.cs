using CoinbaseFetcher.Coinbase.Models;

namespace CoinbaseFetcher.Coinbase.Services.Interfaces;

public interface IOhlcCalculationService
{
    event Action<PeriodData> OnOhlcCompleted;

    void ProcessTick(WebSocketData ticker);
}