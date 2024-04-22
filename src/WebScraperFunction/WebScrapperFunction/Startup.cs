using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Telegram.Bot;
using WebScrapperFunction.Application;
using WebScrapperFunction.Infrastructure;

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

        builder.Services.AddScoped<IJobService, JobService>();
        builder.Services.AddScoped<IMessageBuilder, MessageBuilder>();
        builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(sp => new TelegramBotClient(botToken));
    }
}
