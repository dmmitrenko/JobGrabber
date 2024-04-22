﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Telegram.Bot;
using System.Resources;
using WebScrapperFunction.Application;
using WebScrapperFunction.Application.Cache;
using WebScrapperFunction.Application.Extensions;
using WebScrapperFunction.DataContext.Repositories;
using WebScrapperFunction.Domain.Enums;
using WebScrapperFunction.Infrastructure;
using WebScrapperFunction.Infrastructure.Repositories;
using System.Reflection;

[assembly: FunctionsStartup(typeof(WebScrapperFunction.Startup))]

namespace WebScrapperFunction;

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
        //builder.Services.AddScoped(provider => provider.GetService<IConnectionMultiplexer>().GetDatabase());

        builder.Services.AddScoped<IJobService, JobService>();
        builder.Services.AddScoped<IMessageBuilder, MessageBuilder>();
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        builder.Services.AddScoped<ICacheService, RedisCacheService>();

        builder.Services.ConfigureAutomapper();

        builder.Services.AddScoped(x => new ResourceManager("WebScrapperFunction.Resources.Messages", Assembly.GetExecutingAssembly()));

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
