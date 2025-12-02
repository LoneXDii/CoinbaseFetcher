using System.Net.WebSockets;
using System.Text;
using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Infrastructure.Configuration;
using CoinbaseFetcher.Infrastructure.Services;
using CoinbaseFetcher.Infrastructure.Services.Helpers;
using Microsoft.Extensions.Options;
using Moq;

namespace CoinbaseFetcher.Tests.Infrastructure.Services;

public class WebSocketFetchingServiceTests
{
    private readonly Mock<IWebSocketClient> _webSocketClientMock;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly WebSocketConfiguration _webSocketConfiguration;
    private readonly WebSocketFetchingService _sut;

    public WebSocketFetchingServiceTests()
    {
        _webSocketClientMock = new Mock<IWebSocketClient>();
        _messageBusMock = new Mock<IMessageBus>();
        _webSocketConfiguration = new WebSocketConfiguration
        {
            Channels = ["TestChannel1"],
            Products = ["TestProduct1"],
            Url = "http://TestUrl"
        };
        
        _sut = new WebSocketFetchingService(
            Options.Create(_webSocketConfiguration), 
            _webSocketClientMock.Object,
            _messageBusMock.Object);
    }

    [Fact]
    public async Task ConnectAsync_WhenConnectedAndReceivingData_ShouldSendEventWithCorrectData()
    {
        // Arrange
        var receivedData = Encoding.UTF8.GetBytes("{\"type\":\"ticker\",\"sequence\":117177994892,\"product_id\":\"BTC-USD\",\"price\":\"86937.64\",\"open_24h\":\"86534.6\",\"volume_24h\":\"13524.84294246\",\"low_24h\":\"83800\",\"high_24h\":\"87360\",\"volume_30d\":\"323616.24349789\",\"best_bid\":\"86936.86\",\"best_bid_size\":\"0.08220573\",\"best_ask\":\"86937.64\",\"best_ask_size\":\"0.02425373\",\"side\":\"buy\",\"time\":\"2025-12-02T07:24:28.007175Z\",\"trade_id\":914272439,\"last_size\":\"0.00015457\"}");
        
        SetupWebSocketMock(WebSocketState.Open, receivedData, WebSocketMessageType.Text);
        
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        // Act
        var task = Task.Run(() => _sut.ConnectAsync(cancellationToken));
        await Task.Delay(200);
        cancellationTokenSource.Cancel();
        await task;
        
        // Assert
        _webSocketClientMock
            .Verify(x => x.ConnectAsync(
                It.IsAny<Uri>(),
                cancellationToken),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.SendAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    WebSocketMessageType.Text,
                    true,
                    cancellationToken),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.ReceiveAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    cancellationToken),
                Times.AtLeastOnce);
        
        _messageBusMock.Verify(x => x.SendMessageReceivedEvent(It.IsNotNull<TickData>()), Times.AtLeastOnce);
        _messageBusMock.Verify(x => x.SendMessageReceivedEvent(null), Times.Never);
    }
    
    [Fact]
    public async Task ConnectAsync_WhenConnectedAndReceivingDataWithInvalidType_ShouldSendEventWithNull()
    {
        // Arrange
        var receivedData = Encoding.UTF8.GetBytes("{\"type\":\"invalidType\",\"sequence\":117177994892,\"product_id\":\"BTC-USD\",\"price\":\"86937.64\",\"open_24h\":\"86534.6\",\"volume_24h\":\"13524.84294246\",\"low_24h\":\"83800\",\"high_24h\":\"87360\",\"volume_30d\":\"323616.24349789\",\"best_bid\":\"86936.86\",\"best_bid_size\":\"0.08220573\",\"best_ask\":\"86937.64\",\"best_ask_size\":\"0.02425373\",\"side\":\"buy\",\"time\":\"2025-12-02T07:24:28.007175Z\",\"trade_id\":914272439,\"last_size\":\"0.00015457\"}");
        
        SetupWebSocketMock(WebSocketState.Open, receivedData, WebSocketMessageType.Text);
        
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        // Act
        var task = Task.Run(() => _sut.ConnectAsync(cancellationToken));
        await Task.Delay(200);
        cancellationTokenSource.Cancel();
        await task;
        
        // Assert
        _webSocketClientMock
            .Verify(x => x.ConnectAsync(
                It.IsAny<Uri>(),
                cancellationToken),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.SendAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    WebSocketMessageType.Text,
                    true,
                    cancellationToken),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.ReceiveAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    cancellationToken),
                Times.AtLeastOnce);
        
        _messageBusMock.Verify(x => x.SendMessageReceivedEvent(It.IsNotNull<TickData>()), Times.Never);
        _messageBusMock.Verify(x => x.SendMessageReceivedEvent(null), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task ConnectAsync_WhenMessageTypeIsNotText_ShouldNotSendEvent()
    {
        // Arrange
        var receivedData = Encoding.UTF8.GetBytes("{\"type\":\"ticker\",\"sequence\":117177994892,\"product_id\":\"BTC-USD\",\"price\":\"86937.64\",\"open_24h\":\"86534.6\",\"volume_24h\":\"13524.84294246\",\"low_24h\":\"83800\",\"high_24h\":\"87360\",\"volume_30d\":\"323616.24349789\",\"best_bid\":\"86936.86\",\"best_bid_size\":\"0.08220573\",\"best_ask\":\"86937.64\",\"best_ask_size\":\"0.02425373\",\"side\":\"buy\",\"time\":\"2025-12-02T07:24:28.007175Z\",\"trade_id\":914272439,\"last_size\":\"0.00015457\"}");
        
        SetupWebSocketMock(WebSocketState.Open, receivedData, WebSocketMessageType.Binary);
        
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        // Act
        var task = Task.Run(() => _sut.ConnectAsync(cancellationToken));
        await Task.Delay(200);
        cancellationTokenSource.Cancel();
        await task;
        
        // Assert
        _webSocketClientMock
            .Verify(x => x.ConnectAsync(
                It.IsAny<Uri>(),
                cancellationToken),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.SendAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    WebSocketMessageType.Text,
                    true,
                    cancellationToken),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.ReceiveAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    cancellationToken),
                Times.AtLeastOnce);
        
        _messageBusMock.Verify(x => x.SendMessageReceivedEvent(It.IsAny<TickData>()), Times.Never);
    }
    
    [Fact]
    public async Task ConnectAsync_WhenWebSocketIsNotOpened_ShouldNotSendEventAndReceiveData()
    {
        // Arrange
        var receivedData = Encoding.UTF8.GetBytes("{\"type\":\"ticker\",\"sequence\":117177994892,\"product_id\":\"BTC-USD\",\"price\":\"86937.64\",\"open_24h\":\"86534.6\",\"volume_24h\":\"13524.84294246\",\"low_24h\":\"83800\",\"high_24h\":\"87360\",\"volume_30d\":\"323616.24349789\",\"best_bid\":\"86936.86\",\"best_bid_size\":\"0.08220573\",\"best_ask\":\"86937.64\",\"best_ask_size\":\"0.02425373\",\"side\":\"buy\",\"time\":\"2025-12-02T07:24:28.007175Z\",\"trade_id\":914272439,\"last_size\":\"0.00015457\"}");
        
        SetupWebSocketMock(WebSocketState.Closed, receivedData, WebSocketMessageType.Text);
        
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        // Act
        var task = Task.Run(() => _sut.ConnectAsync(cancellationToken));
        await Task.Delay(200);
        cancellationTokenSource.Cancel();
        await task;
        
        // Assert
        _webSocketClientMock
            .Verify(x => x.ConnectAsync(
                It.IsAny<Uri>(),
                cancellationToken),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.SendAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    WebSocketMessageType.Text,
                    true,
                    cancellationToken),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.ReceiveAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    cancellationToken),
                Times.Never);
        
        _messageBusMock.Verify(x => x.SendMessageReceivedEvent(It.IsAny<TickData>()), Times.Never);
    }

    [Fact]
    public async Task ConnectAsync_WhenCancelled_ShouldFinishFetchingAndNotSendEvent()
    {
        // Arrange
        _webSocketClientMock
            .Setup(x => x.State)
            .Returns(WebSocketState.Open);
        
        _webSocketClientMock
            .Setup(
                x => x.ReceiveAsync(
                    It.IsAny<ArraySegment<byte>>(), 
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        await _sut.ConnectAsync(CancellationToken.None);
        
        // Assert
        _webSocketClientMock
            .Verify(x => x.ConnectAsync(
                    It.IsAny<Uri>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.SendAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    WebSocketMessageType.Text,
                    true,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        _webSocketClientMock
            .Verify(x => x.ReceiveAsync(
                    It.IsAny<ArraySegment<byte>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        _messageBusMock.Verify(x => x.SendMessageReceivedEvent(It.IsAny<TickData>()), Times.Never);
    }
    
    [Fact]
    public void Dispose_WhenCalled_ShouldDisposeWebSocketClient()
    {
        // Act
        _sut.Dispose();
        
        // Assert
        _webSocketClientMock.Verify(x => x.Dispose(), Times.Once);
    }
    
    private void SetupWebSocketMock(
        WebSocketState webSocketState,
        byte[] receivedData,
        WebSocketMessageType webSocketMessageType)
    {
        _webSocketClientMock
            .Setup(x => x.State)
            .Returns(webSocketState);
        
        _webSocketClientMock
            .Setup(x => x.ReceiveAsync(
                It.IsAny<ArraySegment<byte>>(),
                It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> seg, CancellationToken ct) =>
            {
                Buffer.BlockCopy(receivedData, 0, seg.Array!, seg.Offset, receivedData.Length);
            })
            .ReturnsAsync(new WebSocketReceiveResult(receivedData.Length, webSocketMessageType, true));
    }
}
