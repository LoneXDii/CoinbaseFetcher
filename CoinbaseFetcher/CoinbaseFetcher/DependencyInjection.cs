using CoinbaseFetcher.Coinbase.Services.Implementations;
using CoinbaseFetcher.Coinbase.Services.Interfaces;
using CoinbaseFetcher.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoinbaseFetcher;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WebSocketConfiguration>(options => configuration.GetSection("WebSocket").Bind(options));
        
        services.AddSingleton<IWebSocketService, WebSocketService>()
            .AddSingleton<IOhlcCalculationService>(provider => new OhlcCalculationService(TimeSpan.FromMinutes(1)));
        
        return services;
    }
}