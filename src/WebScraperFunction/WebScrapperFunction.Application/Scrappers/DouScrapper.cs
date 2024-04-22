using WebScrapperFunction.Domain.Models;
using WebScrapperFunction.Infrastructure.Scrapers;

namespace WebScrapperFunction.Application.Scrappers;
internal class DouScrapper : IJobScraper
{
    public Task<List<Vacancy>> ScrapeJobs(string specialty, double experience)
    {
        throw new NotImplementedException();
    }
}
