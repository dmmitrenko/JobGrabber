using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text;
using System.Threading;
using WebScrapperFunction.Domain.Models;
using WebScrapperFunction.Infrastructure.Scrapers;

namespace WebScrapperFunction.Application.Scrappers;
public class DjiniScrapper : IJobScraper
{
    private readonly HttpClient _httpClient;

    public DjiniScrapper(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<Vacancy>> ScrapeJobs(string specialty, double experience)
    {
        var response = await GetHtmlContent(specialty, experience);
        if (string.IsNullOrEmpty(response))
        {
            return new List<Vacancy>();
        }

        return GetVacanciesFromHtml(response);
    }

    private async Task<string> GetHtmlContent(string specialty, double experience)
    {
        var query = new StringBuilder("/jobs/?");

        query.Append($"primary_keyword={Uri.EscapeDataString(specialty.ToUpper())}&");
        query.Append($"exp_level={Uri.EscapeDataString(Convert.ToInt16(experience).ToString())}");

        var response = await _httpClient.GetAsync(query.ToString());
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    private List<Vacancy> GetVacanciesFromHtml(string htmlContent)
    {
        var vacancies = new List<Vacancy>();
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        var jobItems = doc.DocumentNode.SelectNodes("//li[contains(@class, 'list-jobs__item')]");

        if (jobItems == null)
            return vacancies;

        foreach (var item in jobItems)
        {
            var titleNode = item.SelectSingleNode(".//a[contains(@class, 'job-list-item__link')]");
            var companyNode = item.SelectSingleNode(".//div[contains(@class, 'd-flex')]/a");
            var locationNode = item.SelectSingleNode(".//span[contains(@class, 'location-text')]");
            var salaryNode = item.SelectSingleNode(".//span[contains(@class, 'public-salary-item')]");
            var postingTimeNode = item.SelectSingleNode(".//span[@title]");
            var additionalInfoNodes = item.SelectNodes(".//div[@class='job-list-item__job-info font-weight-500']/span[@class='nobr']");

            var title = titleNode.InnerText.Trim();
            var vacancyLink = titleNode.Attributes["href"]?.Value;
            var company = companyNode.InnerText.Trim();
            var location = string.Join(" ", locationNode.InnerText.Trim().Split("\n").Select(s => s.Trim()));
            var salary = salaryNode?.InnerText.Trim();
            var postingTimeText = postingTimeNode.Attributes["title"].Value;
            var additionalInfo = string.Join(" · ", additionalInfoNodes?.Select(node => node.InnerText.Trim().Split('·')[1]).ToList());

            DateTime.TryParseExact(postingTimeText, "HH:mm dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var postingTime);

            var vacancy = new Vacancy
            {
                Title = title,
                Company = company,
                Location = location,
                Description = additionalInfo,
                PostedDate = postingTime,
                Link = new Uri(_httpClient.BaseAddress, vacancyLink).ToString(),
                Salary = salary
            };

            vacancies.Add(vacancy);
        }

        return vacancies;

    }
}
