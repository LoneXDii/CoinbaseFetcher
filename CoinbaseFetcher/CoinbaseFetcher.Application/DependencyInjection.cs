using CoinbaseFetcher.Application.Configuration;
using CoinbaseFetcher.Application.Services.Implementations;
using CoinbaseFetcher.Application.Services.Interfaces;
using CoinbaseFetcher.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoinbaseFetcher.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CalculationPeriodConfiguration>(options => configuration.GetSection("CalculationPeriod").Bind(options));

        services.AddSingleton<IMessageBus, MessageBus>();
        services.AddSingleton<IDataProcessingService, DataProcessingService>();
        
        return services;
    }
}
