using UserManagementFunction.Domain.Models;

namespace UserManagementFunction.Infrastructure.Repositories;
public interface ISubscriptionRepository
{
    Task AddSubscription(Subscription subscription, CancellationToken cancellationToken = default);
    Task<List<Subscription>> GetAllSubscriptionsByUserId(long userId, CancellationToken cancellationToken = default);
    Task<Subscription> GetSubscriptionById(long userId, Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<bool> IsUserHasSubscriptionByName(long userId, string subscriptionName, CancellationToken cancellationToken = default);
    Task DeleteSubscription(long userId, string subscriptionName, CancellationToken cancellationToken = default);
}
