using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace EmailSender.Infrastructure.Consumers.Deserializers;

internal class KafkaDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var jsonString = Encoding.UTF8.GetString(data);

        return JsonSerializer.Deserialize<T>(jsonString);
    }
}
