using System.Text;
using WebScrapperFunction.Domain.Models;
using WebScrapperFunction.Infrastructure;

namespace WebScrapperFunction.Application;
public class MessageBuilder : IMessageBuilder
{
    private StringBuilder _builder;

    public IMessageBuilder StartMessage()
    {
        _builder = new StringBuilder();
        return this;
    }

    public IMessageBuilder AddVacancy(Vacancy vacancy)
    {
        throw new NotImplementedException();
    }

    public IMessageBuilder AddVacancies(List<Vacancy> vacancies)
    {
        throw new NotImplementedException();
    }

    public string Build()
    {
        throw new NotImplementedException();
    }
}
