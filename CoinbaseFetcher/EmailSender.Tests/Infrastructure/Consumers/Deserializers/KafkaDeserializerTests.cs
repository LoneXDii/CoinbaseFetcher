using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using EmailSender.Domain.Models;
using EmailSender.Infrastructure.Consumers.Deserializers;
using FluentAssertions;

namespace EmailSender.Tests.Infrastructure.Consumers.Deserializers;

public class KafkaDeserializerTests
{
    [Fact]
    public void Deserialize_ValidUtf8JsonBytes_ReturnsCorrectOhlcDataInstance()
    {
        // Arrange
        var deserializer = new KafkaDeserializer<OhlcData>();
        
        var json = """{"Symbol":"ETH-USDT","Timestamp":"2025-12-02T14:25:30.750Z","Open":3421.123,"High":3450.000,"Low":3400.500,"Close":3444.444,"PeriodStart":"2025-12-02T14:00:00Z"}""";

        var data = Encoding.UTF8.GetBytes(json);
        var span = new ReadOnlySpan<byte>(data);

        // Act
        var result = deserializer.Deserialize(span, isNull: false, new SerializationContext());

        // Assert
        result.Should().NotBeNull();

        var validResult = JsonSerializer.Deserialize<OhlcData>(json);
        result.Should().BeEquivalentTo(validResult);
    }
}
