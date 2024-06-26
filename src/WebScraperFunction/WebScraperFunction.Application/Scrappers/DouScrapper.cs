﻿using WebScraperFunction.Domain.Models;
using WebScraperFunction.Infrastructure.Scrapers;

namespace WebScraperFunction.Application.Scrappers;
internal class DouScrapper : IJobScraper
{
    private readonly HttpClient _httpClient;

    public DouScrapper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<List<Vacancy>> ScrapeJobs(string specialty, double experience, TimeSpan period = default)
    {
        return Task.FromResult(new List<Vacancy>());
    }
}
