using System.Text;
using System.Text.Json;
using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Infrastructure.Producers.Serializers;
using Confluent.Kafka;
using FluentAssertions;

namespace CoinbaseFetcher.Tests.Infrastructure.Producers.Serializers;

public class KafkaSerializerTests
{
    [Fact]
    public void Serialize_WhenPeriodDataProvided_ShouldCreateValidJsonSerialization()
    {
        // Arrange
        var serializer = new KafkaSerializer<PeriodData>();
        
        var data = new PeriodData
        {
            Symbol = "BTC-USDT",
            Timestamp = new DateTime(2025, 12, 2, 15, 30, 45, 123, DateTimeKind.Utc),
            Open = 68500.50m,
            High = 69000.00m,
            Low = 68200.25m,
            Close = 68888.88m,
            PeriodStart = new DateTime(2025, 12, 2, 15, 0, 0, DateTimeKind.Utc),
            PeriodEnd = new DateTime(2025, 12, 2, 15, 59, 59, 999, DateTimeKind.Utc)
        };

        var context = new SerializationContext();

        // Act
        var result = serializer.Serialize(data, context);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty();
        
        var actualJson = Encoding.UTF8.GetString(result);
        var validJson = JsonSerializer.Serialize(data);
        var deserializedModel = JsonSerializer.Deserialize<PeriodData>(actualJson);
        
        actualJson.Should().Be(validJson);
        deserializedModel.Should().BeEquivalentTo(data);
    }
    
    [Fact]
    public void Serialize_TickDataProvided_ShouldCreateValidJsonSerialization()
    {
        // Arrange
        var serializer = new KafkaSerializer<TickData>();
        
        var data = new TickData
        {
            ProductId = "BTC-USDT",
            Price = 123.456m,
            DateTime = new DateTime(2025, 12, 2, 15, 30, 45, 123, DateTimeKind.Utc),
        };

        var context = new SerializationContext();

        // Act
        var result = serializer.Serialize(data, context);

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty();
        
        var actualJson = Encoding.UTF8.GetString(result);
        var validJson = JsonSerializer.Serialize(data);
        var deserializedModel = JsonSerializer.Deserialize<TickData>(actualJson);
        
        actualJson.Should().Be(validJson);
        deserializedModel.Should().BeEquivalentTo(data);
    }
}
