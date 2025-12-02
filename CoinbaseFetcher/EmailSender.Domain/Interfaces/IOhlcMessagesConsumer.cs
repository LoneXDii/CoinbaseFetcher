namespace EmailSender.Domain.Interfaces;

public interface IOhlcMessagesConsumer
{
    Task ConsumeMessagesAsync(CancellationToken cancellationToken);
}
