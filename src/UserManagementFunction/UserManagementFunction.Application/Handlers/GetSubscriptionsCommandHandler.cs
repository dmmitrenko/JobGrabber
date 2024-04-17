using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Models;
using UserManagementFunction.Infrastructure.Repositories;
using UserManagementFunction.Infrastructure.Settings;

namespace UserManagementFunction.Application.Handlers;
public class GetSubscriptionsCommandHandler : ICommandHandler
{
    private readonly GetSubscriptionsCommandSettings _getCommandSettings;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public Domain.Enums.Commands CommandKey => Domain.Enums.Commands.GetSubscriptions;

    public GetSubscriptionsCommandHandler(
        IOptions<GetSubscriptionsCommandSettings> getSubscriptionCommandOptions,
        ISubscriptionRepository subscriptionRepository)
    {
        _getCommandSettings = getSubscriptionCommandOptions.Value;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<CommandResult> HandleCommand(Message message, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        var userId = message.From.Id;

        var subscriptions = await _subscriptionRepository.GetAllSubscriptionsByUserId(userId, cancellationToken);
        return new CommandResult(true, CommandKey, subscriptions);
    }
}
