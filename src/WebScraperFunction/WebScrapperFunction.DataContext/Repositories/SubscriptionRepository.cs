using AutoMapper;
using Azure.Data.Tables;
using WebScrapperFunction.Domain.Models;
using WebScrapperFunction.Infrastructure.Repositories;

namespace WebScrapperFunction.DataContext.Repositories;
public class SubscriptionRepository : ISubscriptionRepository
{
    private const string TableName = nameof(Entities.Subscription);
    private readonly IMapper _mapper;

    public SubscriptionRepository(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<List<Subscription>> GetDefaultSubscriptions()
    {
        var tableClient = await GetTableClient();
        var query = tableClient.QueryAsync<Entities.Subscription>(filter: $"EntityType eq 'Subscription' and IsPremium eq false");

        var subscriptions = new List<Subscription>();

        await foreach (var entity in query)
        {
            subscriptions.Add(_mapper.Map<Subscription>(entity));
        }
        return subscriptions;
    }

    public async Task<List<Subscription>> GetPremiumSubscriptions()
    {
        var tableClient = await GetTableClient();
        var query = tableClient.QueryAsync<Entities.Subscription>(filter: $"EntityType eq 'Subscription' and IsPremium eq true");

        var subscriptions = new List<Subscription>();

        await foreach (var entity in query)
        {
            subscriptions.Add(_mapper.Map<Subscription>(entity));
        }
        return subscriptions;
    }

    public async Task<List<Subscription>> GetAllSubscriptions()
    {
        var tableClient = await GetTableClient();
        var query = tableClient.QueryAsync<Entities.Subscription>(filter: $"EntityType eq 'Subscription'");

        var subscriptions = new List<Subscription>();

        await foreach (var entity in query)
        {
            subscriptions.Add(_mapper.Map<Subscription>(entity));
        }
        return subscriptions;
    }

    private async Task<TableClient> GetTableClient(CancellationToken cancellationToken = default)
    {
        var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Table storage connection string is not configured.");
        }

        var serviceClient = new TableServiceClient(connectionString);

        var tableClient = serviceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync(cancellationToken);
        return tableClient;
    }
}
