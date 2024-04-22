using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using WebScrapperFunction.Infrastructure;

namespace WebScrapperFunction
{
    public class WebScrapperFunction
    {
        private readonly IJobService _jobService;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IMessageBuilder _messageBuilder;

        public WebScrapperFunction(
            IJobService jobService,
            ITelegramBotClient telegramBotClient,
            IMessageBuilder messageBuilder)
        {
            _jobService = jobService;
            _telegramBotClient = telegramBotClient;
            _messageBuilder = messageBuilder;
        }

        [FunctionName("RegularUserJob")]
        public async Task RegularUserRun([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Regular user job fetch executed at: {DateTime.Now}");
            var vacancies = await _jobService.GetJobsForEachSubscription();

            foreach (var vacancy in vacancies)
            {
                foreach (var item in vacancy.Value)
                {
                    var message = _messageBuilder
                    .StartMessage()
                    .AddVacancy(item)
                    .Build();

                    await _telegramBotClient.SendTextMessageAsync(
                        vacancy.Key,
                        message,
                        disableWebPagePreview: true,
                        parseMode: ParseMode.Html);

                }
            }
        }
    }
}
