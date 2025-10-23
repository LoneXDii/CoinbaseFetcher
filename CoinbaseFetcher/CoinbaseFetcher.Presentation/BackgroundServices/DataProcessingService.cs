using CoinbaseFetcher.Application.Services.Interfaces;
using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Domain.Models;
using Microsoft.Extensions.Hosting;

namespace CoinbaseFetcher.Presentation.BackgroundServices;

public class DataProcessingService : BackgroundService
{
    private readonly IDataProcessingService _dataProcessingService;
    private readonly IMessageBus _messageBus;
    private readonly IProducer _producer;

    public DataProcessingService(
        IDataProcessingService dataProcessingService,
        IMessageBus messageBus,
        IProducer producer)
    {
        _dataProcessingService = dataProcessingService;
        _messageBus = messageBus;
        _producer = producer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _dataProcessingService.StartDataProcessing();
        _messageBus.OnPeriodCalculated += ProduceAsync;
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProduceAsync(PeriodData periodData)
    {
        await _producer.ProducePeriodProcessedMessageAsync(periodData, CancellationToken.None);
    }
}
