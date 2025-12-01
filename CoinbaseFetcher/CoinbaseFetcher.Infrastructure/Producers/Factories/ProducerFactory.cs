using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Infrastructure.Producers.Serializers;
using Confluent.Kafka;

namespace CoinbaseFetcher.Infrastructure.Producers.Factories;

internal class ProducerFactory : IProducerFactory
{
    public IProducer<Null, PeriodData> GetProducer(ProducerConfig producerConfig)
    {
        return new ProducerBuilder<Null, PeriodData>(producerConfig)
            .SetValueSerializer(new KafkaSerializer<PeriodData>())
            .Build();
    }
}