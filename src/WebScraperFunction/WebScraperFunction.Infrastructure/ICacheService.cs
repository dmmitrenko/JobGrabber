using WebScraperFunction.Domain.Models;

namespace WebScraperFunction.Infrastructure;
public interface ICacheService
{
    Task<List<Vacancy>> GetCachedVacancies(string key);
    Task SetCachedVacancies(string key, List<Vacancy> vacancies);
}
