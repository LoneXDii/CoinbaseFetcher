using EmailSender.Infrastructure.Services.Factories;
using FluentAssertions;
using MailKit.Net.Smtp;

namespace EmailSender.Tests.Infrastructure.Services.Factories;

public class SmtpClientFactoryTests
{
    private readonly SmtpClientFactory _sut;

    public SmtpClientFactoryTests()
    {
        _sut = new SmtpClientFactory();
    }

    [Fact]
    public void GetSmtpClient_WhenCalled_ShouldReturnSmtpClient()
    {
        // Act
        var result = _sut.GetSmtpClient();
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<SmtpClient>();
    }
}
