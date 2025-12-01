using EmailSender.Infrastructure.Configuration;
using EmailSender.Infrastructure.Services;
using EmailSender.Infrastructure.Services.Factories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;

namespace EmailSender.Tests.Infrastructure.Services;

public class SmtpServiceTests
{
    private readonly SmtpService _sut;
    private readonly Mock<ISmtpClientFactory> _smtpClientFactoryMock;
    private readonly Mock<ISmtpClient> _smtpClientMock;
    private readonly SmtpConfiguration _smtpConfiguration;

    public SmtpServiceTests()
    {
        _smtpClientFactoryMock = new Mock<ISmtpClientFactory>();
        _smtpClientMock = new Mock<ISmtpClient>();
        
        _smtpClientFactoryMock
            .Setup(x => x.GetSmtpClient())
            .Returns(_smtpClientMock.Object);

        _smtpConfiguration = new SmtpConfiguration
        {
            FromEmail = "testFromEmail",
            ToEmail = "testToEmail",
            Host = "testHost",
            Port = 1,
            UserName = "testUser",
            Password = "testPassword",
        };
        
        var options = Options.Create(_smtpConfiguration);
        
        _sut = new SmtpService(options, _smtpClientFactoryMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_WhenCalled_ShouldSendEmailByConfigurationOptions()
    {
        // Arrange
        var message = "TestMessage";
        var subject = "TestSubject";
        
        // Act
        await _sut.SendEmailAsync(message, subject, CancellationToken.None);
        
        // Assert
        _smtpClientFactoryMock.Verify(x => x.GetSmtpClient(), Times.Once);
        
        _smtpClientMock
            .Verify(x => x.ConnectAsync(
                    _smtpConfiguration.Host,
                    _smtpConfiguration.Port,
                    SecureSocketOptions.StartTls,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        _smtpClientMock
            .Verify(x => x.AuthenticateAsync(
                    _smtpConfiguration.UserName,
                    _smtpConfiguration.Password,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        _smtpClientMock
            .Verify(x => x.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    null),
                Times.Once);
        
        _smtpClientMock
            .Verify(x => x.DisconnectAsync(
                    true,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
