using MediatR;
using UserManagementFunction.Application.Commands;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Repositories;

namespace UserManagementFunction.Application.Handlers;
public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionCommand>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public DeleteSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task Handle(DeleteSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var isUserHasSubscription = await _subscriptionRepository.IsUserHasSubscriptionByName(request.UserId, request.SubscriptionTitle, cancellationToken);

        if (!isUserHasSubscription)
        {
            throw new DomainException("That subscription doesn't exist!");
        }

        await _subscriptionRepository.DeleteSubscription(request.UserId, request.SubscriptionTitle, cancellationToken);
    }
}
