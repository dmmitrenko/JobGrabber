using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace UserManagementFunction;

public static class ManagementFunction
{
    [FunctionName("ManagementFunction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var update = JsonConvert.DeserializeObject<Update>(requestBody);

        // Initialize the Telegram Bot client
        var botClient = new TelegramBotClient("");

        if (update?.Message?.Text != null)
        {
            // Echo the received message text
            await botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: $"You said: {update.Message.Text}"
            );
        }

        return new OkResult();
    }
}
