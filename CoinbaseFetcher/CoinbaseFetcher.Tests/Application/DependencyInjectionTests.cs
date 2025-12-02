using CoinbaseFetcher.Application;
using CoinbaseFetcher.Application.Configuration;
using CoinbaseFetcher.Application.Services.Implementations;
using CoinbaseFetcher.Application.Services.Interfaces;
using CoinbaseFetcher.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CoinbaseFetcher.Tests.Application;

public class DependencyInjectionTests
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    private readonly CalculationPeriodConfiguration _calculationPeriodConfiguration;
    
    public DependencyInjectionTests()
    {
        _services = new ServiceCollection();

        _calculationPeriodConfiguration = new CalculationPeriodConfiguration
        {
            CalculationIntervalInMinutes = 123
        };
        
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["CalculationPeriod:CalculationIntervalInMinutes"] = _calculationPeriodConfiguration.CalculationIntervalInMinutes.ToString()
        }!);
        
        _configuration = configurationBuilder.Build();
    }
    
    [Fact]
    public void AddInfrastructure_ShouldRegisterCalculationPeriodConfiguration()
    {
        // Act
        _services.AddApplication(_configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<CalculationPeriodConfiguration>>();
        
        options.Should().NotBeNull();
        options.Value.Should().BeEquivalentTo(_calculationPeriodConfiguration);
    }
    
    [Fact]
    public void AddInfrastructure_ShouldRegisterMessageBusAsSingleton()
    {
        // Act
        _services.AddApplication(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(IMessageBus) && 
            x.ImplementationType == typeof(MessageBus));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
    
    [Fact]
    public void AddInfrastructure_ShouldRegisterDataProcessingServiceAsSingleton()
    {
        // Act
        _services.AddApplication(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(IDataProcessingService) && 
            x.ImplementationType == typeof(DataProcessingService));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
}
