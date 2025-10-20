using CoinbaseFetcher;
using CoinbaseFetcher.Coinbase.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigureApplication(builder.Configuration);
    
builder.Services.AddHostedService<CoinbaseDataProcessingService>();

var host = builder.Build();
await host.RunAsync();