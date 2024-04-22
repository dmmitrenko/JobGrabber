using WebScrapperFunction.Domain.Models;

namespace WebScrapperFunction.Infrastructure;
public interface IMessageBuilder
{
    IMessageBuilder StartMessage();
    IMessageBuilder AddVacancy(Vacancy vacancy);
    IMessageBuilder AddVacancies(List<Vacancy> vacancies);
    string Build();
}
