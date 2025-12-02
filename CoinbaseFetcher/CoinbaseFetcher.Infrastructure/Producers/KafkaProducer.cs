using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Infrastructure.Configuration;
using CoinbaseFetcher.Infrastructure.Producers.Factories;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CoinbaseFetcher.Infrastructure.Producers;

internal class KafkaProducer : IProducer
{
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly IProducerFactory _producerFactory;

    public KafkaProducer(
        IOptions<KafkaConfiguration> kafkaConfiguration,
        IProducerFactory producerFactory)
    {
        _producerFactory = producerFactory;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }
    
    public async Task ProducePeriodProcessedMessageAsync(PeriodData periodData, CancellationToken cancellationToken)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _kafkaConfiguration.Server,
            Acks = Acks.Leader,
        };
        
        using var producer = _producerFactory.GetProducer(producerConfig);

        var message = new Message<Null, PeriodData>()
        {
            Value = periodData
        };
        
        await producer.ProduceAsync(
            _kafkaConfiguration.CoinbaseOhlcTopicName,
            message,
            cancellationToken);
        
        producer.Flush(cancellationToken);
    }
}
