using WebScrapperFunction.Domain.Models;

namespace WebScrapperFunction.Infrastructure;
public interface IJobService
{
    Task<Dictionary<long, List<Vacancy>>> GetJobsForEachSubscription();
}
