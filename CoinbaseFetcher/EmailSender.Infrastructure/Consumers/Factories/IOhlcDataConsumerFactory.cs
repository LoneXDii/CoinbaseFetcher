using Confluent.Kafka;
using EmailSender.Domain.Models;

namespace EmailSender.Infrastructure.Consumers.Factories;

public interface IOhlcDataConsumerFactory
{
    IConsumer<Ignore,OhlcData> GetOhlcDataConsumer(ConsumerConfig config);
}
