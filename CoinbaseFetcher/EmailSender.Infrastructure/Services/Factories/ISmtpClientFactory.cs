using MailKit.Net.Smtp;

namespace EmailSender.Infrastructure.Services.Factories;

public interface ISmtpClientFactory
{
    ISmtpClient GetSmtpClient();
}