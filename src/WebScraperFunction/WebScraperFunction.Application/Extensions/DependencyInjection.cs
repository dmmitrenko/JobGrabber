using Microsoft.Extensions.DependencyInjection;
using WebScraperFunction.Application.MapperProfiles;

namespace WebScraperFunction.Application.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection ConfigureAutomapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(SubscriptionProfile).Assembly);
        return serviceCollection;
    }
}
