using CoinbaseFetcher.Application.Services.Interfaces;
using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;
using CoinbaseFetcher.Presentation.BackgroundServices;
using CoinbaseFetcher.Presentation.Services.Interfaces;
using Moq;

namespace CoinbaseFetcher.Tests.Presentation.BackgroundServices;

public class DataProcessingServiceTests
{
     private readonly Mock<IDataProcessingService> _dataProcessingServiceMock;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly Mock<IProducer> _producerMock;
    private readonly Mock<ITickDataBroadcastService> _tickDataBroadcastServiceMock;
    private readonly DataProcessingService _sut;
    private readonly CancellationTokenSource _cts;

    public DataProcessingServiceTests()
    {
        _dataProcessingServiceMock = new Mock<IDataProcessingService>();
        _messageBusMock = new Mock<IMessageBus>();
        _producerMock = new Mock<IProducer>();
        _tickDataBroadcastServiceMock = new Mock<ITickDataBroadcastService>();
        _cts = new CancellationTokenSource();
        
        _sut = new DataProcessingService(
            _dataProcessingServiceMock.Object,
            _messageBusMock.Object,
            _producerMock.Object,
            _tickDataBroadcastServiceMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceStarts_ShouldStartDataProcessing()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        
        // Act
        await _sut.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        await _sut.StopAsync(CancellationToken.None);
        
        // Assert
        _dataProcessingServiceMock.Verify(
            x => x.StartDataProcessing(),
            Times.Once);
        
        _messageBusMock.VerifyAdd(
            x => x.OnPeriodCalculated += It.IsAny<Func<PeriodData, Task>>(),
            Times.Once);
        
        _messageBusMock.VerifyAdd(
            x => x.OnMessageReceived += It.IsAny<Action<TickData>>(),
            Times.Once);
    }

    [Fact]
    public async Task OnPeriodCalculatedEventHandler_WhenEventIsRaised_ShouldCallProducer()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var periodData = new PeriodData();
        
        // Act
        await _sut.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        _messageBusMock.Raise(x => x.OnPeriodCalculated += null, periodData);
        
        await _sut.StopAsync(CancellationToken.None);
        
        // Assert
        _producerMock.Verify(
            x => x.ProducePeriodProcessedMessageAsync(
                periodData,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task OnMessageReceivedEventHandler_WhenEventIsRaised_ShouldCallBroadcastToClients()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var testMessage = new TickData();
        
        // Act
        await _sut.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        _messageBusMock.Raise(x => x.OnMessageReceived += null, testMessage);
        await _sut.StopAsync(CancellationToken.None);
        
        // Assert
        _tickDataBroadcastServiceMock.Verify(
            x => x.BroadcastToClients(testMessage),
            Times.Once);
    }
}
