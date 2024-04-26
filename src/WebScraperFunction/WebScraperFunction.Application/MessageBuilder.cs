using System.Resources;
using System.Text;
using WebScraperFunction.Domain.Models;
using WebScraperFunction.Infrastructure;

namespace WebScraperFunction.Application;
public class MessageBuilder : IMessageBuilder
{
    private StringBuilder _builder;
    private readonly ResourceManager _resourceManager;

    public MessageBuilder(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public IMessageBuilder StartMessage()
    {
        _builder = new StringBuilder();
        return this;
    }

    public IMessageBuilder AddVacancy(Vacancy vacancy)
    {
        var jobTitle = _resourceManager.GetString("Vacancy_Title");
        var location = _resourceManager.GetString("Vacancy_Location");
        var details = _resourceManager.GetString("Vacancy_Details");
        var postedDate = _resourceManager.GetString("Vacancy_PostedDate");
        var jobLink = _resourceManager.GetString("Vacancy_Link");
        var salary = _resourceManager.GetString("Vacancy_Salary");

        _builder.AppendLine(string.Format(jobTitle, vacancy.Title, vacancy.Company));
        _builder.AppendLine(string.Format(location, vacancy.Location));
        _builder.AppendLine(string.Format(details, vacancy.Description));
        _builder.AppendLine(string.Format(postedDate, vacancy.PostedDate));
        _builder.AppendLine(string.Format(jobLink, vacancy.Link));
        if (!string.IsNullOrEmpty(vacancy.Salary))
        {
            _builder.AppendLine(string.Format(salary, vacancy.Salary));
        }

        return this;
    }

    public IMessageBuilder AddVacancies(List<Vacancy> vacancies)
    {
        foreach (var vacancy in vacancies)
        {
            AddVacancy(vacancy);
        }

        return this;
    }

    public string Build()
    {
        return _builder.ToString();
    }
}
