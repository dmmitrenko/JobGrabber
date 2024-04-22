using Microsoft.Extensions.DependencyInjection;
using WebScrapperFunction.Application.MapperProfiles;

namespace WebScrapperFunction.Application.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection ConfigureAutomapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(SubscriptionProfile).Assembly);
        return serviceCollection;
    }
}
