using MediatR;
using UserManagementFunction.Application.Commands;
using UserManagementFunction.Domain.Models;
using UserManagementFunction.Infrastructure.Repositories;

namespace UserManagementFunction.Application.Handlers;
public class GetSubscriptionsCommandHandler : IRequestHandler<GetSubscriptionsCommand, List<Subscription>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetSubscriptionsCommandHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<List<Subscription>> Handle(GetSubscriptionsCommand request, CancellationToken cancellationToken)
    {
        var subscriptions = await _subscriptionRepository.GetAllSubscriptionsByUserId(request.UserId, cancellationToken);
        return subscriptions;
    }
}
