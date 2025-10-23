using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;

namespace CoinbaseFetcher.Infrastructure.Producers;

public class ConsoleProducer : IProducer
{
    public Task ProducePeriodProcessedMessageAsync(PeriodData periodData, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Produced: open-{periodData.Open} high-{periodData.High} low-{periodData.Low} close-{periodData.Close} timestamp-{periodData.Timestamp.ToString()}");
        return Task.CompletedTask;
    }
}
