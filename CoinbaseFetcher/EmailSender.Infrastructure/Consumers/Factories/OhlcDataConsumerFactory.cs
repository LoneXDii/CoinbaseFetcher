using Confluent.Kafka;
using EmailSender.Domain.Models;
using EmailSender.Infrastructure.Consumers.Deserializers;

namespace EmailSender.Infrastructure.Consumers.Factories;

internal class OhlcDataConsumerFactory : IOhlcDataConsumerFactory
{
    public IConsumer<Ignore, OhlcData> GetOhlcDataConsumer(ConsumerConfig config)
    {
        return new ConsumerBuilder<Ignore, OhlcData>(config)
            .SetValueDeserializer(new KafkaDeserializer<OhlcData>())
            .Build();
    }
}
