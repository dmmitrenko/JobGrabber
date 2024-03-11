using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using UserManagementFunction.Application;
using UserManagementFunction.DataContext.Repositories;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Repositories;

[assembly: FunctionsStartup(typeof(UserManagementFunction.Startup))]

namespace UserManagementFunction;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(sp =>
                new TelegramBotClient("your_bot_token_here"));
        builder.Services.ConfigureMediatR();

        builder.Services.AddScoped<ICommandProcessor, CommandProcessor>();
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        builder.Services.AddAutoMapper(typeof(Startup).Assembly);
    }
}