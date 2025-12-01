using System.Text.Json;
using CoinbaseFetcher.Domain.Interfaces;
using CoinbaseFetcher.Infrastructure;
using CoinbaseFetcher.Infrastructure.Configuration;
using CoinbaseFetcher.Infrastructure.Producers;
using CoinbaseFetcher.Infrastructure.Producers.Factories;
using CoinbaseFetcher.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CoinbaseFetcher.Tests.Infrastructure;

public class DependencyInjectionTests
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    private readonly WebSocketConfiguration _webSocketConfiguration;
    private readonly KafkaConfiguration _kafkaConfiguration;
    
    public DependencyInjectionTests()
    {
        _services = new ServiceCollection();

        _webSocketConfiguration = new WebSocketConfiguration()
        {
            Url = "TestUrl",
            Channels = ["TestChannel1", "TestChannel2"],
            Products = ["TestProduct1", "TestProduct2"],
        };
        
        _kafkaConfiguration = new KafkaConfiguration
        {
            Server = "localhost:9092",
            CoinbaseOhlcTopicName = "testCoinbaseOhlc",
        };
        
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["WebSocket:Url"] =_webSocketConfiguration.Url,
            ["WebSocket:Channels:0"] = "TestChannel1",
            ["WebSocket:Channels:1"] = "TestChannel2",
            ["WebSocket:Products:0"] = "TestProduct1",
            ["WebSocket:Products:1"] = "TestProduct2",
            ["Kafka:Server"] = _kafkaConfiguration.Server,
            ["Kafka:CoinbaseOhlcTopicName"] = _kafkaConfiguration.CoinbaseOhlcTopicName
        }!);
        
        _configuration = configurationBuilder.Build();
    }
    
    [Fact]
    public void AddInfrastructure_ShouldRegisterWebSocketConfiguration()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<WebSocketConfiguration>>();
        
        options.Should().NotBeNull();
        options.Value.Should().BeEquivalentTo(_webSocketConfiguration);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterKafkaConfiguration()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<KafkaConfiguration>>();
        
        options.Should().NotBeNull();
        options.Value.Should().BeEquivalentTo(_kafkaConfiguration);
    }
    
    [Fact]
    public void AddInfrastructure_ShouldRegisterWebSocketFetchingServiceAsSingleton()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(IWebSocketFetchingService) && 
            x.ImplementationType == typeof(WebSocketFetchingService));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
    
    [Fact]
    public void AddInfrastructure_ShouldRegisterKafkaProducerAsSingleton()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(IProducer) && 
            x.ImplementationType == typeof(KafkaProducer));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
    
    [Fact]
    public void AddInfrastructure_ShouldRegisterProducerFactoryAsSingleton()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(IProducerFactory) && 
            x.ImplementationType == typeof(ProducerFactory));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
}