using CoinbaseFetcher.Domain.Models;

namespace CoinbaseFetcher.Domain.Interfaces;

public interface IMessageBus
{
    event Action<TickData> OnMessageReceived;
    event Func<PeriodData, Task> OnPeriodCalculated;
    
    void SendMessageReceivedEvent(TickData? message);
    void SendPeriodDataCalculatedEvent(PeriodData periodData);
}
