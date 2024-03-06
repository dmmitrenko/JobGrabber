using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace UserManagementFunction.Application;
public static class DependencyInjection
{
    public static IServiceCollection ConfigureMediatR(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return serviceCollection;
    }
}
