using CoinbaseFetcher.Application.Services.Implementations;
using CoinbaseFetcher.Domain.Models;
using FluentAssertions;
using Moq;

namespace CoinbaseFetcher.Tests.Application.Services;

public class MessageBusTests
{
    private readonly MessageBus _sut;
    private readonly Mock<Action<TickData>> _mockMessageHandler;
    private readonly Mock<Func<PeriodData, Task>> _mockPeriodHandler;
    
    public MessageBusTests()
    {
        _sut = new MessageBus();
        _mockMessageHandler = new Mock<Action<TickData>>();
        _mockPeriodHandler = new Mock<Func<PeriodData, Task>>();
    }
    
    [Fact]
    public void SendMessageReceivedEvent_WhenMessageIsNull_ShouldNotInvokeEvent()
    {
        // Arrange
        _sut.OnMessageReceived += _mockMessageHandler.Object;

        // Act
        _sut.SendMessageReceivedEvent(null);

        // Assert
        _mockMessageHandler.Verify(h => h(It.IsAny<TickData>()), Times.Never);
    }

    [Fact]
    public void SendMessageReceivedEvent_WhenMessageIsNotNullAndHandlerSubscribed_ShouldInvokeEventWithCorrectMessage()
    {
        // Arrange
        var testTickData = new TickData();
        _sut.OnMessageReceived += _mockMessageHandler.Object;

        // Act
        _sut.SendMessageReceivedEvent(testTickData);

        // Assert
        _mockMessageHandler.Verify(h => h(testTickData), Times.Once);
    }
    
    [Fact]
    public void SendMessageReceivedEvent_WhenNoHandlerSubscribed_ShouldNotThrowException()
    {
        // Act
        var result = () => _sut.SendMessageReceivedEvent(new TickData());

        // Assert
        result.Should().NotThrow();
    }
    
    [Fact]
    public void SendPeriodDataCalculatedEvent_WhenNoHandlerSubscribed_ShouldNotThrowException()
    {
        // Act
        var result = () => _sut.SendPeriodDataCalculatedEvent(new PeriodData());

        // Assert
        result.Should().NotThrow();
    }
    
    [Fact]
    public void SendPeriodDataCalculatedEvent_WhenHandlerSubscribed_ShouldInvokeEventWithCorrectMessage()
    {
        // Arrange
        var testPeriodData = new PeriodData();
        _sut.OnPeriodCalculated += _mockPeriodHandler.Object;

        // Act
        _sut.SendPeriodDataCalculatedEvent(testPeriodData);

        // Assert
        _mockPeriodHandler.Verify(h => h(testPeriodData), Times.Once);
    }
}
