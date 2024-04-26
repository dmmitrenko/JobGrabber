using WebScraperFunction.Domain.Models;

namespace WebScraperFunction.Infrastructure;
public interface IJobService
{
    Task<Dictionary<long, List<Vacancy>>> GetJobsForEachSubscription();
}
