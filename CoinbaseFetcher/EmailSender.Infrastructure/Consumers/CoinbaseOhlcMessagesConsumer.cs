using Confluent.Kafka;
using EmailSender.Application.Services.Interfaces;
using EmailSender.Domain.Interfaces;
using EmailSender.Domain.Models;
using EmailSender.Infrastructure.Configuration;
using EmailSender.Infrastructure.Consumers.Deserializers;
using Microsoft.Extensions.Options;

namespace EmailSender.Infrastructure.Consumers;

internal class CoinbaseOhlcMessagesConsumer : IOhlcMessagesConsumer
{
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly IOhlcNotificationService _ohlcNotificationService;

    public CoinbaseOhlcMessagesConsumer(
        IOhlcNotificationService ohlcNotificationService,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _ohlcNotificationService = ohlcNotificationService;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaConfiguration.Server,
        };
        
        using var consumer = new ConsumerBuilder<Ignore, OhlcData>(consumerConfig)
            .SetValueDeserializer(new KafkaDeserializer<OhlcData>())
            .Build();

        while (!cancellationToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(5));
            
            if (consumeResult is null)
            {
                continue;
            }
            
            await _ohlcNotificationService.SendOhlcEmailNotificationAsync(consumeResult.Message.Value, cancellationToken);
        }
    }
}
