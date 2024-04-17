using Microsoft.Extensions.Options;
using System.Text;
using UserManagementFunction.Domain.Models;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Models;
using UserManagementFunction.Infrastructure.Settings;

namespace UserManagementFunction.Application.Helpers;
public class MessageBuilder : IMessageBuilder 
{
    private readonly AddSubscriptionCommandSettings _addCommand;
    private readonly DeleteSubscriptionCommandSettings _deleteCommand;
    private readonly GetSubscriptionsCommandSettings _getCommand;

    public MessageBuilder(
        IOptions<AddSubscriptionCommandSettings> addSubscriptionCommandSettings,
        IOptions<DeleteSubscriptionCommandSettings> deleteSubscriptionCommandOptions,
        IOptions<GetSubscriptionsCommandSettings> getSubscriptionCommandOptions)
    {
        _addCommand = addSubscriptionCommandSettings.Value;
        _deleteCommand = deleteSubscriptionCommandOptions.Value;
        _getCommand = getSubscriptionCommandOptions.Value;
    }

    public string GetResponseMessage(CommandResult commandResult)
    {
        var messageBuilder = new StringBuilder();

        if (!commandResult.Success)
        {
            messageBuilder.AppendLine(string.Join("\n", commandResult.Errors ?? Enumerable.Empty<string>()));
            messageBuilder.AppendLine($"\nIf you need help feel free to use the <code>-help</code> parameter. For example: <code>{_addCommand.Command} -help</code>");
            return messageBuilder.ToString();
        }

        switch (commandResult.CommandType)
        {
            case Domain.Enums.Commands.AddSubscription:
                messageBuilder.Append("Your subscription has been successfully added! &#10024");
                break;
            case Domain.Enums.Commands.DeleteSubscription:
                messageBuilder.Append("Your subscription has been successfully deleted! &#128076");
                break;
            case Domain.Enums.Commands.GetSubscriptions:
                messageBuilder.Append(GetUserSubscriptionMessage(commandResult.Response as List<Subscription>));
                break;
            default:
                messageBuilder.Append("I don't think I feel well!");
                break;
        }

        return messageBuilder.ToString();
    }

    private string GetUserSubscriptionMessage(List<Domain.Models.Subscription>? subscriptions) 
    {
        if (subscriptions is null)
        {
            return "It seems you don't have any active subscriptions yet...";
        }

        return "Your subscriptions:\n" + string.Join("\n", subscriptions.Select(s => $"&#128073 <code> {s.Title} </code>"));
    } 

    public string AddHelperMessage()
    {
        var helpAddMessage = $"To create a subscription, write for example: <code>{_addCommand.Command} -{_addCommand.TitleParameter} \"middle golang\" " +
            $"-{_addCommand.ExperienceParameter} 2,5 -{_addCommand.SpecialtyParameter} golang </code>\n" +
            $"\n <code>-{_addCommand.ExperienceParameter}</code> parameter to specify experience, " +
            $"\n <code>-{_addCommand.TitleParameter}</code> to name your subscription, " +
            $"\n <code>-{_addCommand.SpecialtyParameter}</code> to specify your technology." +
            $"\n\n<b>note</b>: if your subscription name consists of several words, put them in quotes as shown in the example.";

        return helpAddMessage;
    }

    public string DeleteHelperMessage()
    {
        var helpDeleteMessage = $"To delete a subscription, write for example: <code>{_deleteCommand.Command} -{_deleteCommand.SubscriptionTitleParameter} \"middle golang\"</code>. " +
            $"\nYou can see all your subscription names by using the <code> {_getCommand.Command} </code> command";

        return helpDeleteMessage;
    }
}
