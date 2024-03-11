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

namespace UserManagementFunction;

public class ManagementFunction
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICommandProcessor _commandProcessor;

    public ManagementFunction(ITelegramBotClient telegramBotClient, ICommandProcessor commandProcessor)
    {
        _telegramBotClient = telegramBotClient;
        _commandProcessor = commandProcessor;
    }

    [FunctionName("ManagementFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var update = JsonConvert.DeserializeObject<Update>(requestBody);

        if (update == null)
        {
            log.LogError("Failed to deserialize update.");
            return new BadRequestResult();
        }

        await HandleUpdate(update, CancellationToken.None);

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
        await _commandProcessor.HandleCommand(message);
    }
}
