using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace CoinbaseFetcher.Infrastructure.Producers.Serializers;

internal class KafkaSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
    }
}
