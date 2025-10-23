using EmailSender.Domain.Interfaces;
using Microsoft.Extensions.Hosting;

namespace EmailSender.Presentation.BackgroundServices;

public class ConsumerService : BackgroundService
{
    private readonly IOhlcMessagesConsumer _consumer;

    public ConsumerService(IOhlcMessagesConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.ConsumeMessagesAsync(stoppingToken);
    }
}
