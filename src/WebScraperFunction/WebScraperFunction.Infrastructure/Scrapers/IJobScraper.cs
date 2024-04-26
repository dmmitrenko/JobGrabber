using WebScraperFunction.Domain.Models;

namespace WebScraperFunction.Infrastructure.Scrapers;
public interface IJobScraper
{
    Task<List<Vacancy>> ScrapeJobs(string specialty, double experience, TimeSpan period = default);
}