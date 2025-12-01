using Confluent.Kafka;
using EmailSender.Application.Services.Interfaces;
using EmailSender.Domain.Interfaces;
using EmailSender.Infrastructure.Configuration;
using EmailSender.Infrastructure.Consumers.Factories;
using Microsoft.Extensions.Options;

namespace EmailSender.Infrastructure.Consumers;

internal class CoinbaseOhlcMessagesConsumer : IOhlcMessagesConsumer
{
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly IOhlcNotificationService _ohlcNotificationService;
    private readonly IOhlcDataConsumerFactory _ohlcDataConsumerFactory;

    public CoinbaseOhlcMessagesConsumer(
        IOhlcNotificationService ohlcNotificationService,
        IOhlcDataConsumerFactory ohlcDataConsumerFactory,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _ohlcNotificationService = ohlcNotificationService;
        _ohlcDataConsumerFactory = ohlcDataConsumerFactory;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaConfiguration.Server,
            GroupId = "group"
        };
        
        using var consumer = _ohlcDataConsumerFactory.GetOhlcDataConsumer(consumerConfig);

        consumer.Subscribe(_kafkaConfiguration.CoinbaseOhlcTopicName);
        
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
