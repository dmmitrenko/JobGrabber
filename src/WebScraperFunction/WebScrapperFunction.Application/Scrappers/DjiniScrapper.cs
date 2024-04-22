using WebScrapperFunction.Domain.Models;
using WebScrapperFunction.Infrastructure.Scrapers;

namespace WebScrapperFunction.Application.Scrappers;
public class DjiniScrapper : IJobScraper
{
    public Task<List<Vacancy>> ScrapeJobs(string specialty, double experience)
    {
        throw new NotImplementedException();
    }
}
