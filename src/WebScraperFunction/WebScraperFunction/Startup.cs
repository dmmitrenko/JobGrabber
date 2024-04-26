using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Telegram.Bot;
using System.Resources;
using WebScraperFunction.Application;
using WebScraperFunction.Application.Cache;
using WebScraperFunction.Application.Extensions;
using WebScraperFunction.DataContext.Repositories;
using WebScraperFunction.Domain.Enums;
using WebScraperFunction.Infrastructure;
using WebScraperFunction.Infrastructure.Repositories;
using System.Reflection;
using WebScraperFunction.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

[assembly: FunctionsStartup(typeof(WebScraperFunction.Startup))]

namespace WebScraperFunction;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var botToken = Environment.GetEnvironmentVariable("TelegramBotToken");
        if (string.IsNullOrEmpty(botToken))
        {
            throw new InvalidOperationException("Telegram bot token is not configured.");
        }

        builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(sp => new TelegramBotClient(botToken));

        var redisConnectionString = Environment.GetEnvironmentVariable("RedisConnectionString");
        var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);

        builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        builder.Services.AddSingleton(provider => provider.GetService<IConnectionMultiplexer>().GetDatabase());

        builder.Services.AddScoped<ICacheService, RedisCacheService>();

        builder.Services.AddScoped<IJobService, JobService>();
        builder.Services.AddScoped<IMessageBuilder, MessageBuilder>();
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        builder.Services.AddScoped<ICacheService, RedisCacheService>();

        builder.Services.ConfigureAutomapper();

        builder.Services.AddOptions();

        var configuration = builder.GetContext().Configuration;

        builder.Services.AddOptions<JobSearchingSettings>()
            .Configure(options => configuration.GetSection(nameof(JobSearchingSettings)).Bind(options));

        builder.Services.AddOptions<CacheSettings>()
            .Configure(options => configuration.GetSection(nameof(CacheSettings)).Bind(options));

        builder.Services.AddScoped(x => new ResourceManager("WebScraperFunction.Resources.Messages", Assembly.GetExecutingAssembly()));

        builder.Services.AddHttpClient(JobWebsites.Djini.ToString(), client =>
        {
            client.BaseAddress = new Uri("https://djinni.co");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddHttpClient(JobWebsites.DOU.ToString(), client =>
        {
            client.BaseAddress = new Uri("https://dou.ua");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
    }
}
