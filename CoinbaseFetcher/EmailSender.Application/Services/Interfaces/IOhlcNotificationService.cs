using EmailSender.Domain.Models;

namespace EmailSender.Application.Services.Interfaces;

public interface IOhlcNotificationService
{ 
    Task SendOhlcEmailNotificationAsync(OhlcData ohlcData, CancellationToken cancellationToken);
}
