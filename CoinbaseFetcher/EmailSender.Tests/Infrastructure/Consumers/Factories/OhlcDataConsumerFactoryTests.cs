using Confluent.Kafka;
using EmailSender.Domain.Models;
using EmailSender.Infrastructure.Consumers.Factories;
using FluentAssertions;

namespace EmailSender.Tests.Infrastructure.Consumers.Factories;

public class OhlcDataConsumerFactoryTests
{
    private readonly OhlcDataConsumerFactory _sut;

    public OhlcDataConsumerFactoryTests()
    {
        _sut = new OhlcDataConsumerFactory();
    }

    [Fact]
    public void GetOhlcDataConsumer_WhenCalled_ReturnOhlcDataConsumer()
    {
        // Arrange
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "server",
            GroupId = "group"
        };
        
        // Act
        var result = _sut.GetOhlcDataConsumer(consumerConfig);
        
        // Assert
        result.Should().NotBeNull();
    }
}