using CoinbaseFetcher.Domain.Models;
using Confluent.Kafka;

namespace CoinbaseFetcher.Infrastructure.Producers.Factories;

public interface IProducerFactory
{
    IProducer<Null, PeriodData> GetProducer(ProducerConfig producerConfig);
}
