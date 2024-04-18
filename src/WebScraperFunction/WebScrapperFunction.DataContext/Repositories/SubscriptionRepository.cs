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

    public async Task<Dictionary<string, List<Subscription>>> GetSubscriptionsGroupedByTechnology()
    {
        var tableClient = await GetTableClient();
        var query = tableClient.QueryAsync<Entities.Subscription>(filter: $"EntityType eq 'Subscription'");

        var groupedBySpecialty = new Dictionary<string, List<Subscription>>();

        await foreach (var entity in query)
        {
            if (!groupedBySpecialty.ContainsKey(entity.Specialty))
            {
                groupedBySpecialty[entity.Specialty] = new List<Subscription>();
            }
            groupedBySpecialty[entity.Specialty].Add(_mapper.Map<Subscription>(entity));
        }
        return groupedBySpecialty;
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
