using Microsoft.Extensions.Options;
using System.Globalization;
using Telegram.Bot.Types;
using UserManagementFunction.Domain.Enums;
using UserManagementFunction.Domain.Models;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Models;
using UserManagementFunction.Infrastructure.Repositories;
using UserManagementFunction.Infrastructure.Settings;

namespace UserManagementFunction.Application.Handlers;
public class AddSubscriptionCommandHandler : ICommandHandler
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly AddSubscriptionCommandSettings _addSubscriptionCommand;

    public Domain.Enums.Commands CommandKey => Domain.Enums.Commands.AddSubscription;

    public AddSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IOptions<AddSubscriptionCommandSettings> addSubscriptionCommandOptions)
    {
        _subscriptionRepository = subscriptionRepository;
        _addSubscriptionCommand = addSubscriptionCommandOptions.Value;
    }

    public async Task<CommandResult> HandleCommand(Message message, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        var validationResult = CommandValidator.ValidateAddSubscription(parameters, _addSubscriptionCommand);

        if (!validationResult.IsValid)
        {
            return new CommandResult(false, CommandKey, Errors: validationResult.Errors);
        }

        var subscription = new Subscription
        {
            ChatId = message.Chat.Id,
            UserId = message.From.Id,
            Title = parameters[_addSubscriptionCommand.TitleParameter],
            Specialty = parameters[_addSubscriptionCommand.SpecialtyParameter],
            Experience = Convert.ToDouble(parameters[_addSubscriptionCommand.ExperienceParameter], CultureInfo.InvariantCulture),
            PreferredWebsites = new List<JobWebsites> { JobWebsites.Djini, JobWebsites.DOU },
        };

        await _subscriptionRepository.AddSubscription(subscription, cancellationToken);

        return new CommandResult(true, CommandKey);
    }
}
