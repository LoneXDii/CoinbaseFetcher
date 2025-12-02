using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Infrastructure.Configuration;
using CoinbaseFetcher.Infrastructure.Producers;
using CoinbaseFetcher.Infrastructure.Producers.Factories;
using CoinbaseFetcher.Infrastructure.Services;
using CoinbaseFetcher.Infrastructure.Services.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoinbaseFetcher.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<WebSocketConfiguration>(options => configuration.GetSection("WebSocket").Bind(options))
            .Configure<KafkaConfiguration>(options => configuration.GetSection("Kafka").Bind(options));;
     
        services.AddSingleton<IWebSocketFetchingService, WebSocketFetchingService>();
        services.AddSingleton<IProducer, KafkaProducer>();
        services.AddSingleton<IProducerFactory, ProducerFactory>();
        services.AddSingleton<IWebSocketClient, WebSocketClient>();
        
        return services;
    }
}
