using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Presentation.Services.Implementations;
using CoinbaseFetcher.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace CoinbaseFetcher.Tests.Presentation.Services;

public class TickDataBroadcastServiceTests
{
    private readonly Mock<IHubContext<CoinbaseHub>> _mockHubContext;
    private readonly Mock<IHubClients> _mockClients;
    private readonly Mock<IClientProxy> _mockAll;
    private readonly TickDataBroadcastService _sut;

    public TickDataBroadcastServiceTests()
    {
        _mockAll = new Mock<IClientProxy>();
        _mockAll
            .Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockClients = new Mock<IHubClients>();
        _mockClients.Setup(c => c.All).Returns(_mockAll.Object);

        _mockHubContext = new Mock<IHubContext<CoinbaseHub>>();
        _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);

        _sut = new TickDataBroadcastService(_mockHubContext.Object);
    }

    [Fact]
    public void BroadcastToClients_WhenCalled_ShouldCallSendAsyncWithCorrectParameters()
    {
        // Arrange
        var testTickData = new TickData
        {
            DateTime = DateTime.UtcNow,
            Price = 50000m,
            ProductId = "BTC-USD"
        };

        // Act
        _sut.BroadcastToClients(testTickData);

        // Assert
        object[] sendData = [testTickData];
        
        _mockHubContext
            .Verify(h => h.Clients.All.SendCoreAsync(
                "ReceiveTickData",
                sendData,
                It.IsAny<CancellationToken>()), 
                Times.Once);
    }
}
