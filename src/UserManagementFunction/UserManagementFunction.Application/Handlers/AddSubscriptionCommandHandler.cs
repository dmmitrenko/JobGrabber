using MediatR;
using UserManagementFunction.Application.Commands;
using UserManagementFunction.Infrastructure.Repositories;

namespace UserManagementFunction.Application.Handlers;
public class AddSubscriptionCommandHandler : IRequestHandler<AddSubscriptionCommand>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public AddSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task Handle(AddSubscriptionCommand request, CancellationToken cancellationToken)
    {
        await _subscriptionRepository.AddSubscription(request.Subscription, cancellationToken);
    }
}
