using WebScrapperFunction.Domain.Models;

namespace WebScrapperFunction.Infrastructure;
public interface ICacheService
{
    Task<List<Vacancy>> GetCachedVacancies(string key);
    Task SetCachedVacancies(string key, List<Vacancy> vacancies);
}
