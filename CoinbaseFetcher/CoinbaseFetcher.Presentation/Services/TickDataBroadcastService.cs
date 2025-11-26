using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoinbaseFetcher.Presentation.Services;

public class TickDataBroadcastService
{
    private readonly IHubContext<CoinbaseHub> _hubContext;

    public TickDataBroadcastService(
        IMessageBus messageBus,
        IHubContext<CoinbaseHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async void BroadcastToClients(TickData tickData)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveTickData", tickData);
    }
}
