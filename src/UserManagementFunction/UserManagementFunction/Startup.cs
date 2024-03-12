using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Telegram.Bot;
using UserManagementFunction.Application;
using UserManagementFunction.DataContext.Repositories;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Repositories;
using UserManagementFunction.Infrastructure.Settings;

[assembly: FunctionsStartup(typeof(UserManagementFunction.Startup))]

namespace UserManagementFunction;
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
        builder.Services.ConfigureMediatR();
        builder.Services.ConfigureAutomapper();

        builder.Services.AddScoped<ICommandProcessor, CommandProcessor>();
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        builder.Services.AddOptions();

        var configuration = builder.GetContext().Configuration;

        builder.Services.AddOptions<AddSubscriptionCommandSettings>()
                .Configure(options => configuration.GetSection(nameof(AddSubscriptionCommandSettings)).Bind(options));

        builder.Services.AddOptions<GetSubscriptionsCommandSettings>()
                .Configure(options => configuration.GetSection(nameof(GetSubscriptionsCommandSettings)).Bind(options));

        builder.Services.AddOptions<DeleteSubscriptionCommandSettings>()
                .Configure(options => configuration.GetSection(nameof(DeleteSubscriptionCommandSettings)).Bind(options));

        var value = configuration["GetSubscriptionsCommandSettings:Command"];
        Console.WriteLine($"Config value: {value}");
    }
}