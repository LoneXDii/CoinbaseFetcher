using CoinbaseFetcher.Application;
using CoinbaseFetcher.Infrastructure;
using CoinbaseFetcher.Presentation.Services.Implementations;
using CoinbaseFetcher.Presentation.BackgroundServices;
using CoinbaseFetcher.Presentation.Hubs;
using CoinbaseFetcher.Presentation.Services;
using CoinbaseFetcher.Presentation.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddHostedService<DataFetchingService>();
builder.Services.AddHostedService<DataProcessingService>();
builder.Services.AddSingleton<ITickDataBroadcastService, TickDataBroadcastService>();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(options =>
    options.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
);

app.MapHub<CoinbaseHub>("/coinbase/trades");

await app.RunAsync();
