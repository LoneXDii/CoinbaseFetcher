namespace EmailSender.Domain.Interfaces;

public interface ISmtpService
{
    Task SendEmailAsync(string message, string subject, CancellationToken cancellationToken);
}
