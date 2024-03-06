using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using UserManagementFunction.Application;
using UserManagementFunction.DataContext.Repositories;
using UserManagementFunction.Infrastructure.Repositories;

[assembly: FunctionsStartup(typeof(UserManagementFunction.Startup))]

namespace UserManagementFunction;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.ConfigureMediatR();
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        builder.Services.AddAutoMapper(typeof(Startup).Assembly);
    }
}