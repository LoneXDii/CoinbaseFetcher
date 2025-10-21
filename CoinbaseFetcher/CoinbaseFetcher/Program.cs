using CoinbaseFetcher;
using CoinbaseFetcher.Coinbase.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.ConfigureApplication(builder.Configuration);
    
builder.Services.AddHostedService<CoinbaseDataProcessingService>();

var host = builder.Build();
await host.RunAsync();