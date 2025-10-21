using CoinbaseFetcher.Coinbase.Models;
using CoinbaseFetcher.Configuration;
using CoinbaseFetcher.Email.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CoinbaseFetcher.Email.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly SmtpConfiguration _smtpConfiguration;

    public EmailService(IOptions<SmtpConfiguration> smtpConfiguration)
    {
        _smtpConfiguration = smtpConfiguration.Value;
    }
    
    public void SendEmail(PeriodData periodData)
    {
        var message = new MimeMessage();
        
        message.From.Add(MailboxAddress.Parse(_smtpConfiguration.FromEmail));
        message.To.Add(MailboxAddress.Parse(_smtpConfiguration.ToEmail));
        message.Subject = "BTC-USD OHLC";
        message.Body = BuildEmailBody(periodData);

        var smtp = new SmtpClient();
        
        smtp.Connect(_smtpConfiguration.Host, _smtpConfiguration.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_smtpConfiguration.UserName, _smtpConfiguration.Password);
        smtp.Send(message);
        
        smtp.Disconnect(true);
    }

    private MimeEntity BuildEmailBody(PeriodData periodData)
    {
        var builder = new BodyBuilder
        {
            HtmlBody = $"""
                        <h1>BTC-USD OHLC for {periodData.Timestamp.ToString()} UTC</h1>
                        <h2>Open: {periodData.Open}</h2>
                        <h2>High: {periodData.High}</h2>
                        <h2>Low: {periodData.Low}</h2>
                        <h2>Close: {periodData.Close}</h2>
                        <p>Period start: {periodData.PeriodStart.ToString()} UTC</p>
                        <p>Period end: {periodData.PeriodEnd.ToString()} UTC</p>
                        """
        };

        return builder.ToMessageBody();
    }
}