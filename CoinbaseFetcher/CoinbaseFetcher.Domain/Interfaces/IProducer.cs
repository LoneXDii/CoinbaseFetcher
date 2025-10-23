using CoinbaseFetcher.Domain.Models;

namespace CoinbaseFetcher.Domain.Interfaces;

public interface IProducer
{
    Task ProducePeriodProcessedMessageAsync(PeriodData periodData, CancellationToken cancellationToken);
}
