using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using UserManagementFunction.Application.Commands;
using UserManagementFunction.Domain.Enums;
using UserManagementFunction.Domain.Models;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Settings;

namespace UserManagementFunction.Application;
public class CommandProcessor : ICommandProcessor
{
    private Dictionary<string, Func<Message, Task<string>>> _commandHandlers;
    private readonly IMediator _mediator;
    private readonly DeleteSubscriptionCommandSettings _deleteSubscriptionCommand;
    private readonly GetSubscriptionsCommandSettings _getSubscriptionsCommand;
    private readonly AddSubscriptionCommandSettings _addSubscriptionOptions;

    public CommandProcessor(
        IMediator mediator, 
        IOptions<AddSubscriptionCommandSettings> addSubscriptionCommand,
        IOptions<DeleteSubscriptionCommandSettings> deleteSubscriptionCommand,
        IOptions<GetSubscriptionsCommandSettings> getSubscriptionsCommand)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _deleteSubscriptionCommand = deleteSubscriptionCommand.Value;
        _getSubscriptionsCommand = getSubscriptionsCommand.Value;
        _addSubscriptionOptions = addSubscriptionCommand.Value;

        _commandHandlers = new Dictionary<string, Func<Message, Task<string>>>
        {
            { _addSubscriptionOptions.Command, HandleAddSubscriptionCommand },
            { _deleteSubscriptionCommand.Command, HandleDeleteSubscriptionCommand },
            { _getSubscriptionsCommand.Command, HandleGetSubscriptionsCommand },
        };
    }

    public async Task<string> HandleCommand(Message message, CancellationToken cancellationToken = default)
    {
        var text = message.Text;
        var command = text.Split(new[] { ' ' })[0];

        if (!_commandHandlers.TryGetValue(command, out var handler))
        {
            throw new DomainException("This command doesn't exist. ");
        }

        return await handler(message);
    }

    private async Task<string> HandleGetSubscriptionsCommand(Message message)
    {
        var userId = message.From.Id;

        var command = new GetSubscriptionsCommand
        {
            UserId = userId,
        };

        var subscriptions = await _mediator.Send(command);
        return "Your subscriptions:\n" + string.Join("\n", subscriptions.Select(s => $"- [{s.Title}] {s.Specialty}, {s.Experience}"));
    }
    private async Task<string> HandleDeleteSubscriptionCommand(Message message)
    {
        var parameters = ParseParameters(message.Text);
        var validationResult = CommandValidator.ValidateDeleteSubscription(parameters, _deleteSubscriptionCommand);

        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join("\n", validationResult.Errors));
        }

        var command = new DeleteSubscriptionCommand
        {
            UserId = message.From.Id,
            SubscriptionTitle = parameters[_deleteSubscriptionCommand.SubscriptionTitleParameter]
        };

        await _mediator.Send(command);
        return "Your subscription has been successfully deleted!";
    }

    private async Task<string> HandleAddSubscriptionCommand(Message message)
    {
        var parameters = ParseParameters(message.Text);
        var validationResult = CommandValidator.ValidateAddSubscription(parameters, _addSubscriptionOptions);

        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join("\n", validationResult.Errors));
        }

        var command = new AddSubscriptionCommand
        {
            Subscription = new Subscription
            {
                UserId = message.From.Id,
                Title = parameters[_addSubscriptionOptions.TitleParameter],
                Specialty = parameters[_addSubscriptionOptions.SpecialtyParameter],
                Experience = Convert.ToDouble(parameters[_addSubscriptionOptions.ExperienceParameter]),
                PreferredWebsites = new List<JobWebsites> { JobWebsites.Djini, JobWebsites.DOU },
            }
        };

        await _mediator.Send(command);

        return "Your subscription has been successfully added!";
    }

    private Dictionary<string, string> ParseParameters(string messageText)
    {
        return messageText.Split(' ').Skip(1)
             .Select(part => part.Split(':'))
             .Where(part => part.Length == 2)
             .ToDictionary(split => split[0], split => split[1], StringComparer.OrdinalIgnoreCase);
    }
}
