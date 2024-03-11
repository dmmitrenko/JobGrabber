using MediatR;
using Telegram.Bot.Types;
using UserManagementFunction.Application.Commands;
using UserManagementFunction.Domain.Enums;
using UserManagementFunction.Domain.Models;
using UserManagementFunction.Infrastructure;

namespace UserManagementFunction.Application;
public class CommandProcessor : ICommandProcessor
{
    private Dictionary<string, Func<Message, Task>> _commandHandlers;
    private readonly IMediator _mediator;

    public CommandProcessor(IMediator mediator)
    {
        _commandHandlers = new Dictionary<string, Func<Message, Task>>
        {
            { nameof(Domain.Enums.Commands.AddSubscription), HandleAddSubscriptionCommand },
            { nameof(Domain.Enums.Commands.DeleteSubscription), HandleDeleteSubscriptionCommand },
            { nameof(Domain.Enums.Commands.GetSubscriptions), HandleGetSubscriptionsCommand },
        };
        _mediator = mediator;
    }

    public async Task HandleCommand(Message message)
    {
        var text = message.Text;

        if (!text.StartsWith("/"))
        {
            throw new DomainException("Input must start with a slash '/'.");
        }

        var command = text.Split(new[] { ' ' })[0];

        if (_commandHandlers.TryGetValue(command, out var handler))
        {
            await handler(message);
        }
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
        var requiredParameters = new List<string>()
        {
            nameof(Subscription.Title),
            nameof(Subscription.Keyword),
            nameof(Subscription.Experience),
            nameof(Subscription.EnglishLevel)
        };


        if (!IsValidationSuccess(parameters, requiredParameters))
        {
            throw new DomainException();
        }


        var subscription = new Subscription
        {
            UserId = message.From.Id,
            Title = parameters[nameof(Subscription.Title)],
            Keyword = parameters[nameof(Subscription.Keyword)],
            Experience = Convert.ToDouble(parameters[nameof(Subscription.Experience)]),
            EnglishLevel = Enum.Parse<EnglishLevels>(parameters[nameof(Subscription.EnglishLevel)])
        };

        var command = new AddSubscriptionCommand
        {
            Subscription = subscription
        };

        await _mediator.Send(command);
    }

    private Dictionary<string, string> ParseParameters(string messageText)
    {
        var parts = messageText.Split(' ', 2);
        var allParameters = parts[1];

        var parameters = allParameters
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('='))
            .Where(part => part.Length == 2)
            .ToDictionary(split => split[0], split => split[1], StringComparer.OrdinalIgnoreCase);

        return parameters;
    }

    private bool IsValidationSuccess(Dictionary<string, string> parameters, List<string> requiredParameters)
    {
        foreach (var paramName in requiredParameters)
        {
            if (!parameters.TryGetValue(paramName, out var value) || string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
        }

        return true;
    }
}
