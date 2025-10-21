using CoinbaseFetcher.Coinbase.Models;

namespace CoinbaseFetcher.Email.Services.Interfaces;

public interface IEmailService
{
    void SendEmail(PeriodData periodData);
}