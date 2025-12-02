using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Infrastructure.Configuration;
using CoinbaseFetcher.Infrastructure.Producers;
using CoinbaseFetcher.Infrastructure.Producers.Factories;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Moq;

namespace CoinbaseFetcher.Tests.Infrastructure.Producers;

public class KafkaProducerTests
{
    private readonly Mock<IProducerFactory> _producerFactoryMock;
    private readonly Mock<IProducer<Null, PeriodData>> _producerMock;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly KafkaProducer _sut;

    public KafkaProducerTests()
    {
        _producerFactoryMock = new Mock<IProducerFactory>();
        _producerMock = new Mock<IProducer<Null, PeriodData>>();
        
        _producerFactoryMock
            .Setup(x => x.GetProducer(It.IsAny<ProducerConfig>()))
            .Returns(_producerMock.Object);
        
        _kafkaConfiguration = new KafkaConfiguration
        {
            CoinbaseOhlcTopicName = "TestTopic",
            Server = "TestServer"
        };
        
        _sut = new KafkaProducer(
            Options.Create(_kafkaConfiguration),
            _producerFactoryMock.Object);
    }

    [Fact]
    public async Task ProducePeriodProcessedMessageAsync_WhenCalled_ShouldProduceMessage()
    {
        // Arrange
        var periodData = new PeriodData();
        
        // Act
        await _sut.ProducePeriodProcessedMessageAsync(periodData, CancellationToken.None);
        
        // Assert
        _producerFactoryMock
            .Verify(
                x => x.GetProducer(It.IsAny<ProducerConfig>()),
                Times.Once);
        
        _producerMock
            .Verify(
                x => x.ProduceAsync(
                    _kafkaConfiguration.CoinbaseOhlcTopicName,
                    It.IsAny<Message<Null, PeriodData>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        _producerMock
            .Verify(
                x => x.Flush(It.IsAny<CancellationToken>()),
                Times.Once);
    }
}