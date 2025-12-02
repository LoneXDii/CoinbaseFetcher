using EmailSender.Application.Services.Implementations;
using EmailSender.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EmailSender.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IOhlcNotificationService, OhlcNotificationService>();
        
        return services;
    }
}
