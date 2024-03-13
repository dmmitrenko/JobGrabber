using MediatR;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.RegularExpressions;
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
            return BuildAddHelpMessage();
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
        return "Your subscriptions:\n" + string.Join("\n", subscriptions.Select(s => $"&#128073 <code> {s.Title} </code>"));
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
        return "Your subscription has been successfully deleted! &#128076";
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
                Experience = Convert.ToDouble(parameters[_addSubscriptionOptions.ExperienceParameter], CultureInfo.InvariantCulture),
                PreferredWebsites = new List<JobWebsites> { JobWebsites.Djini, JobWebsites.DOU },
            }
        };

        await _mediator.Send(command);

        return "Your subscription has been successfully added! &#10024";
    }

    private Dictionary<string, string> ParseParameters(string messageText)
    {
        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var parts = Regex.Matches(messageText, @"(-\w+)\s+(?:""(.+?)""|(\S+))")
            .Cast<Match>()
            .Select(m => new { Key = m.Groups[1].Value.TrimStart('-'), Value = m.Groups[2].Success ? m.Groups[2].Value : m.Groups[3].Value });

        foreach (var part in parts)
        {
            parameters[part.Key] = part.Value;
        }

        return parameters;
    }

    private string BuildAddHelpMessage()
    {
        var helpAddMessage = $"To create a subscription, write for example: <code>{_addSubscriptionOptions.Command} -{_addSubscriptionOptions.TitleParameter} \"middle golang\" " +
            $"-{_addSubscriptionOptions.ExperienceParameter} 2,5 -{_addSubscriptionOptions.SpecialtyParameter} golang </code>\n" +
            $"\n <code>-{_addSubscriptionOptions.ExperienceParameter}</code> parameter to specify experience, " +
            $"\n <code>-{_addSubscriptionOptions.TitleParameter}</code> to name your subscription, " +
            $"\n <code>-{_addSubscriptionOptions.SpecialtyParameter}</code> to specify your technology." +
            $"\n\n note: if your subscription name consists of several words, put them in quotes as shown in the example.";

        return helpAddMessage;
    }
}
