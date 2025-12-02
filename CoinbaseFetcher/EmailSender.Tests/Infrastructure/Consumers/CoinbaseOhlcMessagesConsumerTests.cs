using Confluent.Kafka;
using EmailSender.Application.Services.Interfaces;
using EmailSender.Domain.Models;
using EmailSender.Infrastructure.Configuration;
using EmailSender.Infrastructure.Consumers;
using EmailSender.Infrastructure.Consumers.Factories;
using Microsoft.Extensions.Options;
using Moq;

namespace EmailSender.Tests.Infrastructure.Consumers;

public class CoinbaseOhlcMessagesConsumerTests
{
    private readonly Mock<IOhlcNotificationService> _ohlcNotificationServiceMock;
    private readonly Mock<IOhlcDataConsumerFactory> _ohlcDataConsumerFactoryMock;
    private readonly Mock<IConsumer<Ignore, OhlcData>> _consumerMock;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly CoinbaseOhlcMessagesConsumer _sut;

    public CoinbaseOhlcMessagesConsumerTests()
    {
        _ohlcNotificationServiceMock = new Mock<IOhlcNotificationService>();
        _ohlcDataConsumerFactoryMock = new Mock<IOhlcDataConsumerFactory>();
        _consumerMock = new Mock<IConsumer<Ignore, OhlcData>>();
        
        _ohlcDataConsumerFactoryMock
            .Setup(x => x.GetOhlcDataConsumer(It.IsAny<ConsumerConfig>()))
            .Returns(_consumerMock.Object);

        _kafkaConfiguration = new KafkaConfiguration
        {
            CoinbaseOhlcTopicName = "TestTopicName",
            Server = "TestServer"
        };
        
        _sut = new CoinbaseOhlcMessagesConsumer(
            _ohlcNotificationServiceMock.Object,
            _ohlcDataConsumerFactoryMock.Object,
            Options.Create(_kafkaConfiguration));
    }

    [Fact]
    public async Task ConsumeMessagesAsync_WhenNoMessages_ShouldNotSendNotification()
    {
        // Arrange
        _consumerMock
            .Setup(x => x.Consume(It.IsAny<TimeSpan>()))
            .Returns((ConsumeResult<Ignore, OhlcData>)null);
        
        var cancellationTokenSource = new CancellationTokenSource();
        var stoppingToken = cancellationTokenSource.Token;
        
        // Act
        var task = Task.Run(() => _sut.ConsumeMessagesAsync(stoppingToken)); 
        await Task.Delay(100);
        cancellationTokenSource.Cancel();
        await task;
        
        // Assert
        _ohlcDataConsumerFactoryMock
            .Verify(
                x => x.GetOhlcDataConsumer(It.IsAny<ConsumerConfig>()),
                Times.Once);
        
        _consumerMock
            .Verify(
                x => x.Subscribe(_kafkaConfiguration.CoinbaseOhlcTopicName),
                Times.Once);
        
        _consumerMock
            .Verify(
                x => x.Consume(It.IsAny<TimeSpan>()),
                Times.AtLeastOnce);
        
        _ohlcNotificationServiceMock
            .Verify(
                x => x.SendOhlcEmailNotificationAsync(
                    It.IsAny<OhlcData>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }
    
    [Fact]
    public async Task ConsumeMessagesAsync_WhenValidMessages_ShouldSendNotification()
    {
        // Arrange
        var message = new ConsumeResult<Ignore, OhlcData>
        {
            Message = new Message<Ignore, OhlcData>
            {
                Value = new OhlcData()
            }
        };

        _consumerMock
            .Setup(x => x.Consume(It.IsAny<TimeSpan>()))
            .Returns(message);
        
        var cancellationTokenSource = new CancellationTokenSource();
        var stoppingToken = cancellationTokenSource.Token;
        
        // Act
        var task = Task.Run(() => _sut.ConsumeMessagesAsync(stoppingToken)); 
        await Task.Delay(100);
        cancellationTokenSource.Cancel();
        await task;
        
        // Assert
        _ohlcDataConsumerFactoryMock
            .Verify(
                x => x.GetOhlcDataConsumer(It.IsAny<ConsumerConfig>()),
                Times.Once);
        
        _consumerMock
            .Verify(
                x => x.Subscribe(_kafkaConfiguration.CoinbaseOhlcTopicName),
                Times.Once);
        
        _consumerMock
            .Verify(
                x => x.Consume(It.IsAny<TimeSpan>()),
                Times.AtLeastOnce);
        
        _ohlcNotificationServiceMock
            .Verify(
                x => x.SendOhlcEmailNotificationAsync(
                    message.Message.Value,
                    It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);
    }
}
