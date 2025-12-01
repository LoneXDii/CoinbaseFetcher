using EmailSender.Domain.Interfaces;
using EmailSender.Infrastructure;
using EmailSender.Infrastructure.Configuration;
using EmailSender.Infrastructure.Consumers;
using EmailSender.Infrastructure.Consumers.Factories;
using EmailSender.Infrastructure.Services;
using EmailSender.Infrastructure.Services.Factories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EmailSender.Tests.Infrastructure;

public class DependencyInjectionTests
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    private readonly SmtpConfiguration _smtpConfiguration;
    private readonly KafkaConfiguration _kafkaConfiguration;
    
    public DependencyInjectionTests()
    {
        _services = new ServiceCollection();

        _smtpConfiguration = new SmtpConfiguration
        {
            Host = "test.smtp.com",
            Port = 587,
            UserName = "testuser",
            Password = "testpass",
            FromEmail = "testFromEmail",
            ToEmail = "testToEmail",
        };

        _kafkaConfiguration = new KafkaConfiguration
        {
            Server = "localhost:9092",
            CoinbaseOhlcTopicName = "testCoinbaseOhlc",
        };
        
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Smtp:Host"] = _smtpConfiguration.Host,
            ["Smtp:Port"] = _smtpConfiguration.Port.ToString(),
            ["Smtp:Username"] = _smtpConfiguration.UserName,
            ["Smtp:Password"] = _smtpConfiguration.Password,
            ["Smtp:FromEmail"] = _smtpConfiguration.FromEmail,
            ["Smtp:ToEmail"] = _smtpConfiguration.ToEmail,
            ["Kafka:Server"] = _kafkaConfiguration.Server,
            ["Kafka:CoinbaseOhlcTopicName"] = _kafkaConfiguration.CoinbaseOhlcTopicName
        }!);
        
        _configuration = configurationBuilder.Build();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterSmtpConfiguration()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<SmtpConfiguration>>();
        
        options.Should().NotBeNull();
        options.Value.Host.Should().Be(_smtpConfiguration.Host);
        options.Value.Port.Should().Be(_smtpConfiguration.Port);
        options.Value.UserName.Should().Be(_smtpConfiguration.UserName);
        options.Value.Password.Should().Be(_smtpConfiguration.Password);
        options.Value.FromEmail.Should().Be(_smtpConfiguration.FromEmail);
        options.Value.ToEmail.Should().Be(_smtpConfiguration.ToEmail);
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
        options.Value.Server.Should().Be(_kafkaConfiguration.Server);
        options.Value.CoinbaseOhlcTopicName.Should().Be(_kafkaConfiguration.CoinbaseOhlcTopicName);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterSmtpServiceAsSingleton()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(ISmtpService) && 
            x.ImplementationType == typeof(SmtpService));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterCoinbaseOhlcMessagesConsumerAsSingleton()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(IOhlcMessagesConsumer) && 
            x.ImplementationType == typeof(CoinbaseOhlcMessagesConsumer));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterSmtpClientFactoryAsSingleton()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(ISmtpClientFactory) && 
            x.ImplementationType == typeof(SmtpClientFactory));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterOhlcDataConsumerFactoryAsScoped()
    {
        // Act
        _services.AddInfrastructure(_configuration);

        // Assert
        var descriptor = _services.FirstOrDefault(x => 
            x.ServiceType == typeof(IOhlcDataConsumerFactory) && 
            x.ImplementationType == typeof(OhlcDataConsumerFactory));
        
        descriptor.Should().NotBeNull();
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }
}
