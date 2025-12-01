using CoinbaseFetcher.Infrastructure.Producers.Factories;
using Confluent.Kafka;
using FluentAssertions;

namespace CoinbaseFetcher.Tests.Infrastructure.Producers.Factories;

public class ProducerFactoryTests
{
    private readonly ProducerFactory _sut;

    public ProducerFactoryTests()
    {
        _sut = new ProducerFactory();
    }

    [Fact]
    public void GetProducer_WhenCalled_ShouldReturnProducer()
    {
        // Arrange
        var producerConfig = new ProducerConfig()
        {
            BootstrapServers = "server",
            Acks = Acks.Leader,
        };
        
        // Act
        var result = _sut.GetProducer(producerConfig);
        
        // Assert
        result.Should().NotBeNull();
    }
}