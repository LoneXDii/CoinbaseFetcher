using EmailSender.Domain.Interfaces;
using EmailSender.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailSender.Infrastructure.Services;

internal class SmtpService : ISmtpService
{
    private readonly SmtpConfiguration _smtpConfiguration;

    public SmtpService(IOptions<SmtpConfiguration> smtpConfiguration)
    {
        _smtpConfiguration = smtpConfiguration.Value;
    }

    public async Task SendEmailAsync(string message, string subject, CancellationToken cancellationToken)
    {
        var mimeMessage = new MimeMessage();
        
        mimeMessage.From.Add(MailboxAddress.Parse(_smtpConfiguration.FromEmail));
        mimeMessage.To.Add(MailboxAddress.Parse(_smtpConfiguration.ToEmail));
        mimeMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message
        };
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(
            _smtpConfiguration.Host,
            _smtpConfiguration.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);
        
        await smtpClient.AuthenticateAsync(
            _smtpConfiguration.UserName,
            _smtpConfiguration.Password,
            cancellationToken);
        
        await smtpClient.SendAsync(mimeMessage, cancellationToken);
        
        await smtpClient.DisconnectAsync(true, cancellationToken);
    }
}
