using Castle.Core.Smtp;
using EmailSender.Domain.Interfaces;
using EmailSender.Presentation.BackgroundServices;
using Moq;

namespace EmailSender.Tests.Presentation.BackgroundServices;

public class ConsumerServiceTests
{
    private readonly Mock<IOhlcMessagesConsumer> _consumerMock;
    private readonly ConsumerService _sut;

    public ConsumerServiceTests()
    {
        _consumerMock = new Mock<IOhlcMessagesConsumer>();
        _sut = new ConsumerService(_consumerMock.Object);
    }
    
    [Fact]
    public async Task ExecuteAsync_WhenServiceStarts_ShouldStartConsumer()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        
        // Act
        await _sut.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        await _sut.StopAsync(CancellationToken.None);
        
        // Assert
        _consumerMock.Verify(
            x => x.ConsumeMessagesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
