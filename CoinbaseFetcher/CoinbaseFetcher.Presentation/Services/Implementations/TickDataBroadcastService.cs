using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Presentation.Hubs;
using CoinbaseFetcher.Presentation.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CoinbaseFetcher.Presentation.Services.Implementations;

public class TickDataBroadcastService : ITickDataBroadcastService
{
    private readonly IHubContext<CoinbaseHub> _hubContext;

    public TickDataBroadcastService(
        IHubContext<CoinbaseHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async void BroadcastToClients(TickData tickData)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveTickData", tickData);
    }
}
