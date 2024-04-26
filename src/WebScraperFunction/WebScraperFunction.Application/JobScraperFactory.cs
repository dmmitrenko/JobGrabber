using WebScraperFunction.Application.Scrappers;
using WebScraperFunction.Domain.Enums;
using WebScraperFunction.Infrastructure.Scrapers;

namespace WebScraperFunction.Application;
public static class JobScraperFactory
{
    public static IJobScraper GetScraper(JobWebsites website, IHttpClientFactory httpClientFactory)
    {
        switch (website)
        {
            case JobWebsites.Djini:
                return new DjiniScrapper(httpClientFactory.CreateClient(JobWebsites.Djini.ToString()));
            case JobWebsites.DOU:
                return new DouScrapper(httpClientFactory.CreateClient(JobWebsites.DOU.ToString()));
            default:
                throw new ArgumentException("Invalid job site");
        }
    }
}
