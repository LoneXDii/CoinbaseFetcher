using EmailSender.Application.Services.Implementations;
using EmailSender.Domain.Interfaces;
using EmailSender.Domain.Models;
using Moq;

namespace EmailSender.Tests.Application.Services;

public class OhlcNotificationServiceTests
{
    private readonly OhlcNotificationService _sut;
    private readonly Mock<ISmtpService> _smtpServiceMock;

    public OhlcNotificationServiceTests()
    {
        _smtpServiceMock = new Mock<ISmtpService>();
        
        _sut = new OhlcNotificationService(_smtpServiceMock.Object);
    }

    [Fact]
    public async Task SendOhlcEmailNotificationAsync_WhenCalled_ShouldSendEmail()
    {
        // Arrange
        var ohlcData = new OhlcData
        {
            Open = 1,
            High = 2,
            Low = 3,
            Close = 4,
            PeriodStart = DateTime.Now,
            Symbol = "TEST",
            Timestamp = DateTime.Now,
        };
        
        // Act
        await _sut.SendOhlcEmailNotificationAsync(ohlcData, CancellationToken.None);
        
        // Assert
        _smtpServiceMock
            .Verify(x => x.SendEmailAsync(
                It.IsAny<string>(),
                $"{ohlcData.Symbol} OHLC",
                It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
