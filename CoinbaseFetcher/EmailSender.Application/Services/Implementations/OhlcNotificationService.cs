using EmailSender.Application.Services.Interfaces;
using EmailSender.Domain.Interfaces;
using EmailSender.Domain.Models;

namespace EmailSender.Application.Services.Implementations;

internal class OhlcNotificationService : IOhlcNotificationService
{
    private readonly ISmtpService _smtpService;

    public OhlcNotificationService(ISmtpService smtpService)
    {
        _smtpService = smtpService;
    }

    public async Task SendOhlcEmailNotificationAsync(OhlcData ohlcData, CancellationToken cancellationToken)
    {
        var emailBody = BuildEmailHtmlBody(ohlcData);
        var emailSubject = $"{ohlcData.Symbol} OHLC";
        
        await _smtpService.SendEmailAsync(emailBody, emailSubject, cancellationToken);
    }

    private string BuildEmailHtmlBody(OhlcData ohlcData)
    {
        return $"""
                <h1>BTC-USD OHLC for {ohlcData.Timestamp.ToString()} UTC</h1>
                <h2>Open: {ohlcData.Open}</h2>
                <h2>High: {ohlcData.High}</h2>
                <h2>Low: {ohlcData.Low}</h2>
                <h2>Close: {ohlcData.Close}</h2>
                <p>Period start: {ohlcData.PeriodStart.ToString()} UTC</p>
                <p>Period end: {ohlcData.Timestamp.ToString()} UTC</p>
                """;
    }
}
