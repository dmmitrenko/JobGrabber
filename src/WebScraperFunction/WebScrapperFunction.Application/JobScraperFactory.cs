using WebScrapperFunction.Application.Scrappers;
using WebScrapperFunction.Domain.Enums;
using WebScrapperFunction.Infrastructure.Scrapers;

namespace WebScrapperFunction.Application;
public static class JobScraperFactory
{
    public static IJobScraper GetScraper(JobWebsites website, IHttpClientFactory httpClientFactory)
    {
        switch (website)
        {
            case JobWebsites.Djini:
                return new DjiniScrapper();
            case JobWebsites.DOU:
                return new DouScrapper();
            default:
                throw new ArgumentException("Invalid job site");
        }
    }
}
