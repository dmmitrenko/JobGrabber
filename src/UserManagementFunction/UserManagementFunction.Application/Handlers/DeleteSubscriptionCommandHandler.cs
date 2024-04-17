using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using UserManagementFunction.Domain.Enums;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Models;
using UserManagementFunction.Infrastructure.Repositories;
using UserManagementFunction.Infrastructure.Settings;

namespace UserManagementFunction.Application.Handlers;
public class DeleteSubscriptionCommandHandler : ICommandHandler
{
    private readonly DeleteSubscriptionCommandSettings _deleteCommandSettings;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public Domain.Enums.Commands CommandKey => Domain.Enums.Commands.DeleteSubscription;


    public DeleteSubscriptionCommandHandler(
        IOptions<DeleteSubscriptionCommandSettings> deleteSubscriptionoptions,
        ISubscriptionRepository subscriptionRepository)
    {
        _deleteCommandSettings = deleteSubscriptionoptions.Value;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<CommandResult> HandleCommand(Message message, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        var validationResult = CommandValidator.ValidateDeleteSubscription(parameters, _deleteCommandSettings);

        if (!validationResult.IsValid)
        {
            return new CommandResult(false, CommandKey, Errors: validationResult.Errors);
        }

        var userId = message.From.Id;
        var subscriptionTitle = parameters[_deleteCommandSettings.SubscriptionTitleParameter];

        var isUserHasSubscription = await _subscriptionRepository.IsUserHasSubscriptionByName(userId, subscriptionTitle, cancellationToken);
        if (!isUserHasSubscription)
        {
            throw new DomainException("That subscription doesn't exist!");
        }

        await _subscriptionRepository.DeleteSubscription(userId, subscriptionTitle, cancellationToken);
        return new CommandResult(true, CommandKey);
    }
}
