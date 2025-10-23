using CoinbaseFetcher.Application;
using CoinbaseFetcher.Infrastructure;
using CoinbaseFetcher.Presentation.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddHostedService<DataFetchingService>();
builder.Services.AddHostedService<DataProcessingService>();

var host = builder.Build();
await host.RunAsync();
