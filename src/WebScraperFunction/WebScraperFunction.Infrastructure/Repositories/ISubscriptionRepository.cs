using WebScraperFunction.Domain.Models;

namespace WebScraperFunction.Infrastructure.Repositories;
public interface ISubscriptionRepository
{
    Task<List<Subscription>> GetAllSubscriptions();
    Task<List<Subscription>> GetDefaultSubscriptions();
    Task<List<Subscription>> GetPremiumSubscriptions();
}