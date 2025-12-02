using CoinbaseFetcher.Domain.Models;

namespace CoinbaseFetcher.Presentation.Services.Interfaces;

public interface ITickDataBroadcastService
{
    void BroadcastToClients(TickData tickData);
}
