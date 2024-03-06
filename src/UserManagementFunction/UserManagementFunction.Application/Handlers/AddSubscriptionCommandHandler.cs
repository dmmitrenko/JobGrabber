using MediatR;
using UserManagementFunction.Application.Commands;
using UserManagementFunction.Domain.Enums;
using UserManagementFunction.Domain.Models;
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
        // TODO: remove stub
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            Keyword = ".NET",
            PreferredWebsites = new List<JobWebsites> { JobWebsites.Djini },
            UserId = Guid.NewGuid(),
            Experience = 3d,
            EnglishLevel = Domain.Enums.EnglishLevels.Advanced
        };
        await _subscriptionRepository.AddSubscription(subscription, cancellationToken);
    }
}
