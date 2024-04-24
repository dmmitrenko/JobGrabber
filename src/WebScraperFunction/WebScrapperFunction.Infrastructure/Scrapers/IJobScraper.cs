using WebScrapperFunction.Domain.Models;

namespace WebScrapperFunction.Infrastructure.Scrapers;
public interface IJobScraper
{
    Task<List<Vacancy>> ScrapeJobs(string specialty, double experience, TimeSpan period = default);
}