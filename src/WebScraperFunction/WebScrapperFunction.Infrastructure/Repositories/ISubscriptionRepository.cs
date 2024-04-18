using WebScrapperFunction.Domain.Models;

namespace WebScrapperFunction.Infrastructure.Repositories;
public interface ISubscriptionRepository
{
    Task<Dictionary<string, List<Subscription>>> GetSubscriptionsGroupedByTechnology();
}