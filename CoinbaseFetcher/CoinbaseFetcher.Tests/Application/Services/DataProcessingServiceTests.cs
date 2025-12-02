using CoinbaseFetcher.Application.Configuration;
using CoinbaseFetcher.Application.Services.Implementations;
using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace CoinbaseFetcher.Tests.Application.Services;

public class DataProcessingServiceTests
{
    private readonly Mock<IMessageBus> _mockMessageBus;
    private readonly CalculationPeriodConfiguration _config;
    private readonly DataProcessingService _sut;

    public DataProcessingServiceTests()
    {
        _mockMessageBus = new Mock<IMessageBus>();
        _config = new CalculationPeriodConfiguration { CalculationIntervalInMinutes = 5 };
        var options = Options.Create(_config);
        _sut = new DataProcessingService(_mockMessageBus.Object, options);
    }

    [Fact]
    public void StartDataProcessing_WhenProcessedFirstTickInPeriod_ShouldNotSendEvent()
    {
        // Arrange
        Action<TickData>? capturedHandler = null;
        _mockMessageBus
            .SetupAdd(b => b.OnMessageReceived += It.IsAny<Action<TickData>>())
            .Callback<Action<TickData>>(handler => capturedHandler = handler);

        var now = DateTime.UtcNow;
        
        var periodStart = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            0,
            DateTimeKind.Utc);
        
        var tickData = new TickData
        {
            DateTime = periodStart.AddSeconds(10),
            Price = 50000m
        };

        // Act
        _sut.StartDataProcessing();
        capturedHandler?.Invoke(tickData);

        // Assert
        _mockMessageBus.Verify(b => b.SendPeriodDataCalculatedEvent(It.IsAny<PeriodData>()), Times.Never);
    }

    [Fact]
    public void StartDataProcessing_WhenProcessedSubsequentTicksInSamePeriod_ShouldNotSendEvent()
    {
        // Arrange
        Action<TickData>? capturedHandler = null;
        _mockMessageBus
            .SetupAdd(b => b.OnMessageReceived += It.IsAny<Action<TickData>>())
            .Callback<Action<TickData>>(handler => capturedHandler = handler);

        var now = DateTime.UtcNow;
        
        var periodStart = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            0,
            DateTimeKind.Utc);
        
        var firstTick = new TickData
        {
            DateTime = periodStart.AddSeconds(10),
            Price = 50000m
        };
        
        var secondTick = new TickData
        {
            DateTime = periodStart.AddSeconds(30),
            Price = 51000m
        };

        // Act
        _sut.StartDataProcessing();
        capturedHandler!(firstTick);
        capturedHandler(secondTick);

        // Assert
        _mockMessageBus.Verify(b => b.SendPeriodDataCalculatedEvent(It.IsAny<PeriodData>()), Times.Never);
    }

    [Fact]
    public void StartDataProcessing_WhenProcessedTickThatStartsNewPeriod_ShouldSendEventForPreviousPeriod()
    {
        // Arrange
        Action<TickData>? capturedHandler = null;
        _mockMessageBus
            .SetupAdd(b => b.OnMessageReceived += It.IsAny<Action<TickData>>())
            .Callback<Action<TickData>>(handler => capturedHandler = handler);

        var now = DateTime.UtcNow;
        var periodStart = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            0,
            DateTimeKind.Utc);
        
        var tickInOldPeriod = new TickData
        {
            DateTime = periodStart.AddMinutes(1),
            Price = 50000m
        };
        
        var newPeriodTick = new TickData
        {
            DateTime = periodStart.AddMinutes(5),
            Price = 52000m
        };

        // Act
        _sut.StartDataProcessing();
        capturedHandler!(tickInOldPeriod);
        capturedHandler(newPeriodTick);

        // Assert
        _mockMessageBus.Verify(b => b.SendPeriodDataCalculatedEvent(
            It.Is<PeriodData>(p =>
                p.Symbol == "BTC-USD" &&
                p.Open == 50000m &&
                p.High == 50000m &&
                p.Low == 50000m &&
                p.Close == 50000m &&
                p.PeriodStart == periodStart &&
                p.PeriodEnd == periodStart.AddMinutes(5)
            )), Times.Once);
    }

    [Fact]
    public void StartDataProcessing_WhenProcessedMultiplePeriods_ShouldSendEventsForEachCompletedPeriod()
    {
        // Arrange
        Action<TickData>? capturedHandler = null;
        _mockMessageBus
            .SetupAdd(b => b.OnMessageReceived += It.IsAny<Action<TickData>>())
            .Callback<Action<TickData>>(handler => capturedHandler = handler);

        var now = DateTime.UtcNow;
        
        var firstPeriodStart = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            0,
            DateTimeKind.Utc);
        
        var tick1 = new TickData
        {
            DateTime = firstPeriodStart.AddMinutes(1),
            Price = 50000m
        };
        
        var newPeriod1Tick = new TickData
        {
            DateTime = firstPeriodStart.AddMinutes(5),
            Price = 51000m
        };

        var secondPeriodStart = new DateTime(
            newPeriod1Tick.DateTime.Year,
            newPeriod1Tick.DateTime.Month,
            newPeriod1Tick.DateTime.Day,
            newPeriod1Tick.DateTime.Hour,
            newPeriod1Tick.DateTime.Minute,
            0,
            DateTimeKind.Utc);
        
        var tick2 = new TickData
        {
            DateTime = secondPeriodStart.AddMinutes(1),
            Price = 52000m
        };
        
        var newPeriod2Tick = new TickData
        {
            DateTime = secondPeriodStart.AddMinutes(5),
            Price = 53000m
        };

        // Act
        _sut.StartDataProcessing();
        capturedHandler!(tick1);
        capturedHandler(newPeriod1Tick);
        capturedHandler(tick2);
        capturedHandler(newPeriod2Tick);

        // Assert
        _mockMessageBus
            .Verify(
                b => b.SendPeriodDataCalculatedEvent(It.Is<PeriodData>(p => p.Close == 50000m)),
                Times.Once);
        _mockMessageBus
            .Verify(
                b => b.SendPeriodDataCalculatedEvent(It.Is<PeriodData>(p => p.Close == 52000m)),
                Times.Once);
        _mockMessageBus
            .Verify(
                b => b.SendPeriodDataCalculatedEvent(It.IsAny<PeriodData>()),
                Times.Exactly(2));
    }

    [Fact]
    public void StartDataProcessing_WhenProcessedTickWithSeconds_ShouldTruncatePeriodStartToMinute()
    {
        // Arrange
        Action<TickData>? capturedHandler = null;
        _mockMessageBus
            .SetupAdd(b => b.OnMessageReceived += It.IsAny<Action<TickData>>())
            .Callback<Action<TickData>>(handler => capturedHandler = handler);

        var now = DateTime.UtcNow;
        var initStart = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            0,
            DateTimeKind.Utc);
        
        var tickInOld = new TickData
        {
            DateTime = initStart.AddMinutes(1),
            Price = 50000m
        };
        
        var triggerTickWithSeconds = new TickData
        {
            DateTime = initStart.AddMinutes(5).AddSeconds(30),
            Price = 51000m
        };
        var boundaryTick = new TickData
        {
            DateTime = initStart.AddMinutes(10).AddSeconds(30),
            Price = 52000m
        };

        var truncatedStart = new DateTime(
            triggerTickWithSeconds.DateTime.Year,
            triggerTickWithSeconds.DateTime.Month,
            triggerTickWithSeconds.DateTime.Day,
            triggerTickWithSeconds.DateTime.Hour,
            triggerTickWithSeconds.DateTime.Minute,
            0,
            DateTimeKind.Utc);

        // Act
        _sut.StartDataProcessing();
        capturedHandler!(tickInOld);
        capturedHandler(triggerTickWithSeconds);
        capturedHandler(boundaryTick);

        // Assert
        _mockMessageBus.Verify(b => b.SendPeriodDataCalculatedEvent(
            It.Is<PeriodData>(p =>
                p.PeriodStart == truncatedStart
            )), Times.Once);
    }
}
