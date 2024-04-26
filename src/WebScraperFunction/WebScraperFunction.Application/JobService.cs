using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using WebScraperFunction.Domain.Enums;
using WebScraperFunction.Domain.Models;
using WebScraperFunction.Infrastructure;
using WebScraperFunction.Infrastructure.Repositories;
using WebScraperFunction.Infrastructure.Settings;

namespace WebScraperFunction.Application;
public class JobService : IJobService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICacheService _cacheService;
    private readonly JobSearchingSettings _jobSearchingSettings;

    public JobService(
        ISubscriptionRepository subscriptionRepository,
        IHttpClientFactory httpClientFactory,
        IOptions<JobSearchingSettings> jobSearchingSettings,
        ICacheService cacheService)
    {
        _subscriptionRepository = subscriptionRepository;
        _httpClientFactory = httpClientFactory;
        _cacheService = cacheService;
        _jobSearchingSettings = jobSearchingSettings.Value;
    }

    public async Task<Dictionary<long, List<Vacancy>>> GetJobsForEachSubscription()
    {
        var subscriptions = await _subscriptionRepository.GetAllSubscriptions();
        var jobsForChats = new ConcurrentDictionary<long, List<Vacancy>>();
        var tasks = subscriptions.Select(subscription => ProcessSubscription(subscription, jobsForChats));

        await Task.WhenAll(tasks);

        return jobsForChats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private async Task ProcessSubscription(Subscription subscription, ConcurrentDictionary<long, List<Vacancy>> jobsForChats)
    {
        var vacancies = new List<Vacancy>();

        var scrapeTasks = subscription.PreferredWebsites.Select(website =>
            FetchVacancies(subscription, website));
        var results = await Task.WhenAll(scrapeTasks);

        foreach (var result in results)
        {
            vacancies.AddRange(result);
        }

        jobsForChats.AddOrUpdate(subscription.ChatId, vacancies, (key, existingVal) => {
            existingVal.AddRange(vacancies);
            return existingVal;
        });
    }

    private async Task<List<Vacancy>> FetchVacancies(Subscription subscription, JobWebsites website)
    {
        string cacheKey = $"{subscription.Specialty}-{subscription.Experience}-{website}";
        var cachedVacancies = await _cacheService.GetCachedVacancies(cacheKey);

        if (cachedVacancies.Any())
        {
            return cachedVacancies;
        }

        var scraper = JobScraperFactory.GetScraper(website, _httpClientFactory);
        var scrapedVacancies = await scraper.ScrapeJobs(subscription.Specialty, subscription.Experience, _jobSearchingSettings.DefaultCheckInterval);
        await _cacheService.SetCachedVacancies(cacheKey, scrapedVacancies);

        return scrapedVacancies;
    }
}
