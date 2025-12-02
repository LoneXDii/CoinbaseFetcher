using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;

namespace CoinbaseFetcher.Application.Services.Implementations;

internal class MessageBus : IMessageBus
{
    public event Action<TickData>? OnMessageReceived;
    public event Func<PeriodData, Task>? OnPeriodCalculated;
    
    public void SendMessageReceivedEvent(TickData? message)
    {
        if (message is null)
        {
            return;
        }
        
        OnMessageReceived?.Invoke(message);
    }

    public void SendPeriodDataCalculatedEvent(PeriodData periodData)
    {
        OnPeriodCalculated?.Invoke(periodData);
    }
}
