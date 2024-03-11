using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserManagementFunction.Application.MapperProfiles;

namespace UserManagementFunction.Application;
public static class DependencyInjection
{
    public static IServiceCollection ConfigureMediatR(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return serviceCollection;
    }

    public static IServiceCollection ConfigureAutomapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(SubscriptionProfile).Assembly);
        return serviceCollection;
    }
}
