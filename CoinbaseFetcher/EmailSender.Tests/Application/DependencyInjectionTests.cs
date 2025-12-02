using EmailSender.Application;
using EmailSender.Application.Services.Implementations;
using EmailSender.Application.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EmailSender.Tests.Application;

public class DependencyInjectionTests
{
    private readonly IServiceCollection _services;

    public DependencyInjectionTests()
    {
        _services = new ServiceCollection();
    }
    
    [Fact]
    public void AddApplication_ShouldRegisterOhlcNotificationServiceAsSingleton()
    {
        // Act
        _services.AddApplication();

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(IOhlcNotificationService) && 
            x.ImplementationType == typeof(OhlcNotificationService));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
}
