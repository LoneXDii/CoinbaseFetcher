using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Presentation.BackgroundServices;
using FluentAssertions;
using Moq;

namespace CoinbaseFetcher.Tests.Presentation.BackgroundServices;

public class DataFetchingServiceTests
{
    private readonly Mock<IWebSocketFetchingService> _webSocketServiceMock;
    private readonly DataFetchingService _sut;

    public DataFetchingServiceTests()
    {
        _webSocketServiceMock = new Mock<IWebSocketFetchingService>();
        _sut = new DataFetchingService(_webSocketServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceStarts_ShouldCallConnectAsync()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var stoppingToken = cancellationTokenSource.Token;
        
        var connectTaskCompletionSource = new TaskCompletionSource<bool>();
        
        _webSocketServiceMock
            .Setup(x => x.ConnectAsync(It.IsAny<CancellationToken>()))
            .Returns(connectTaskCompletionSource.Task);

        // Act
        var startTask = _sut.StartAsync(stoppingToken);
        
        await Task.Delay(100);
        
        connectTaskCompletionSource.SetResult(true);
        
        await _sut.StopAsync(CancellationToken.None);
        await startTask;

        // Assert
        _webSocketServiceMock.Verify(
            x => x.ConnectAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Dispose_ShouldDisposeWebSocketService()
    {
        // Act
        _sut.Dispose();

        // Assert
        _webSocketServiceMock.Verify(
            x => x.Dispose(),
            Times.Once);
    }
}
