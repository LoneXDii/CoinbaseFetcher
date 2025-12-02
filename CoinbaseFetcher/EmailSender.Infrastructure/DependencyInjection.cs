using EmailSender.Domain.Interfaces;
using EmailSender.Infrastructure.Configuration;
using EmailSender.Infrastructure.Consumers;
using EmailSender.Infrastructure.Consumers.Factories;
using EmailSender.Infrastructure.Services;
using EmailSender.Infrastructure.Services.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailSender.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<SmtpConfiguration>(options => configuration.GetSection("Smtp").Bind(options))
            .Configure<KafkaConfiguration>(options => configuration.GetSection("Kafka").Bind(options));
        
        services.AddSingleton<ISmtpService, SmtpService>();
        services.AddSingleton<IOhlcMessagesConsumer, CoinbaseOhlcMessagesConsumer>();
        services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();
        services.AddScoped<IOhlcDataConsumerFactory, OhlcDataConsumerFactory>();
        
        return services;
    }
}
