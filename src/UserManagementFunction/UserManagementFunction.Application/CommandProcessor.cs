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
    private Dictionary<string, Func<Message, Task>> _commandHandlers;
    private readonly IMediator _mediator;
    private readonly AddSubscriptionCommandSettings _addSubscriptionOptions;

    public CommandProcessor(
        IMediator mediator, 
        IOptions<AddSubscriptionCommandSettings> addSubscriptionOptions)
    {
        _commandHandlers = new Dictionary<string, Func<Message, Task>>
        {
            { _addSubscriptionOptions.Command, HandleAddSubscriptionCommand },
            { nameof(Domain.Enums.Commands.DeleteSubscription), HandleDeleteSubscriptionCommand },
            { nameof(Domain.Enums.Commands.GetSubscriptions), HandleGetSubscriptionsCommand },
        };
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _addSubscriptionOptions = addSubscriptionOptions.Value;
    }

    public async Task HandleCommand(Message message)
    {
        var text = message.Text;
        var command = text.Split(new[] { ' ' })[0];

        if (!_commandHandlers.TryGetValue(command, out var handler))
        {
            throw new DomainException("There is no such command.");
        }

        await handler(message);
    }

    private Task HandleGetSubscriptionsCommand(Message message)
    {
        throw new NotImplementedException();
    }
    private Task HandleDeleteSubscriptionCommand(Message message)
    {
        throw new NotImplementedException();
    }

    private async Task HandleAddSubscriptionCommand(Message message)
    {
        var parameters = ParseParameters(message.Text);
        var validationResult = CommandValidator.ValidateAddSubscription(parameters);

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
    }

    private Dictionary<string, string> ParseParameters(string messageText)
    {
        return messageText.Split(' ').Skip(1)
             .Select(part => part.Split('='))
             .Where(part => part.Length == 2)
             .ToDictionary(split => split[0], split => split[1], StringComparer.OrdinalIgnoreCase);
    }
}
