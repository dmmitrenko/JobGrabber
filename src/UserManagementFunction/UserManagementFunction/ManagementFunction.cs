using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot;
using System;
using System.Threading;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.IO;
using UserManagementFunction.Infrastructure;
using System.Collections.Generic;
using UserManagementFunction.Domain.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace UserManagementFunction;

public class ManagementFunction
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICommandProcessor _commandProcessor;
    private readonly IMessageBuilder _messageBuilder;
    private readonly Dictionary<string, Commands> CommandMappings = new Dictionary<string, Commands>
    {
        { "/add", Commands.AddSubscription },
        { "/delete", Commands.DeleteSubscription },
        { "/list", Commands.GetSubscriptions }
    };

    public ManagementFunction(
        ITelegramBotClient telegramBotClient,
        ICommandProcessor commandProcessor,
        IMessageBuilder messageBuilder)
    {
        _telegramBotClient = telegramBotClient;
        _commandProcessor = commandProcessor;
        _messageBuilder = messageBuilder;
    }

    [FunctionName("ManagementFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        log.LogInformation(requestBody);

        var update = JsonConvert.DeserializeObject<Update>(requestBody);

        if (update == null)
        {
            log.LogError("Failed to deserialize update.");
            return new BadRequestResult();
        }

        try
        {
            await HandleUpdate(update, CancellationToken.None);

            return new OkResult();
        }
        catch (DomainException ex)
        {
            await _telegramBotClient.SendTextMessageAsync(
               update.Message.Chat.Id,
               $"{ex.Message} &#129430;",
               disableWebPagePreview: true,
               parseMode: ParseMode.Html);
        }
        catch (Exception)
        {
            await _telegramBotClient.SendTextMessageAsync(
               update.Message.Chat.Id,
               $"Something went wrong &#129430;",
               disableWebPagePreview: true,
               parseMode: ParseMode.Html);
        }

        return new OkResult();

    }

    private async Task HandleUpdate(Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            UpdateType.Message or
            UpdateType.EditedMessage => BotOnMessageReceived(update.Message, cancellationToken),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery, cancellationToken),
            UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery, cancellationToken),
            UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task BotOnChosenInlineResultReceived(ChosenInlineResult? chosenInlineResult, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task BotOnInlineQueryReceived(InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task BotOnCallbackQueryReceived(CallbackQuery? callbackQuery, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task BotOnMessageReceived(Message? message, CancellationToken cancellationToken)
    {
        var command = message.Text.Split(new[] { ' ' })[0];
        var isHelpNeeded = message.Text.Contains("-help"); 

        if (!CommandMappings.TryGetValue(command, out var parsedCommand))
        {
            var helpAddMessage = _messageBuilder.GetCommandHelp(Commands.AddSubscription);
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                helpAddMessage,
                disableWebPagePreview: true,
                parseMode: ParseMode.Html);

            return;
        }

        if (isHelpNeeded)
        {
            var helpMessage = _messageBuilder.GetCommandHelp(parsedCommand);
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                helpMessage,
                disableWebPagePreview: true,
                parseMode: ParseMode.Html);

            return;
        }

        var response = await _commandProcessor.HandleCommand(message, parsedCommand, cancellationToken);
        var responseMessage = _messageBuilder.GetResponseMessage(response);

        await _telegramBotClient.SendTextMessageAsync(
            message.Chat.Id,
            responseMessage,
            disableWebPagePreview: true,
            parseMode: ParseMode.Html);
    }
}
