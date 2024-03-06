using UserManagementFunction.Domain.Models;

namespace UserManagementFunction.Infrastructure.Repositories;
public interface ISubscriptionRepository
{
    Task AddSubscription(Subscription subscription, CancellationToken cancellationToken = default);
    Task<List<Subscription>> GetAllSubscriptionsByUserId(Guid userId, CancellationToken cancellationToken = default);
    Task<Subscription> GetSubscriptionById(Guid userId, Guid subscriptionId, CancellationToken cancellationToken = default);
}
