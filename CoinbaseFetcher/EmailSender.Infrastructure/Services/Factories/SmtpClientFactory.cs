using MailKit.Net.Smtp;

namespace EmailSender.Infrastructure.Services.Factories;

internal class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient GetSmtpClient()
    {
        return new SmtpClient();
    }
}