using AutoMapper;
using Azure;
using Azure.Data.Tables;
using System.Threading;
using Telegram.Bot.Types;
using UserManagementFunction.DataContext.Entities;
using UserManagementFunction.Infrastructure;
using UserManagementFunction.Infrastructure.Repositories;

namespace UserManagementFunction.DataContext.Repositories;
public class SubscriptionRepository : ISubscriptionRepository
{
    private const string TableName = nameof(Subscription);
    private readonly IMapper _mapper;

    public SubscriptionRepository(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task AddSubscription(Domain.Models.Subscription subscriptionModel, CancellationToken cancellationToken = default)
    {
        var tableClient = await GetTableClient(cancellationToken);
        try
        {
            var subscription = _mapper.Map<Subscription>(subscriptionModel);
            await tableClient.UpsertEntityAsync(subscription);
        }
        catch (Exception)
        {
            throw;
        }
        
    }

    public async Task DeleteSubscription(long userId, string subscriptionName, CancellationToken cancellationToken = default)
    {
        var tableClient = await GetTableClient(cancellationToken);
        var partitionKey = userId.ToString();
        var rowKey = subscriptionName;

        try
        {
            await tableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            throw new DomainException("Nothing to delete.");
        }
        catch (Exception)
        {
            throw new DomainException();
        }
    }

    public async Task<List<Domain.Models.Subscription>> GetAllSubscriptionsByUserId(long userId, CancellationToken cancellationToken = default)
    {
        var tableClient = await GetTableClient(cancellationToken);

        string filter = $"PartitionKey eq '{userId}'";


        List<Subscription> subscriptions = new List<Subscription>();

        await foreach (var entity in tableClient.QueryAsync<TableEntity>(filter: filter, cancellationToken: cancellationToken))
        {
            subscriptions.Add(ConvertToSubscription(entity));
        }

        return _mapper.Map<List<Domain.Models.Subscription>>(subscriptions);
    }

    public async Task<Domain.Models.Subscription> GetSubscriptionById(long userId, Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        var tableClient = await GetTableClient(cancellationToken);
        var subscription = await tableClient.GetEntityAsync<Subscription>(userId.ToString(), subscriptionId.ToString());

        return _mapper.Map<Domain.Models.Subscription>(subscription);

    }

    public async Task<bool> IsUserHasSubscriptionByName(long userId, string subscriptionName, CancellationToken cancellationToken = default)
    {
        var tableClient = await GetTableClient(cancellationToken);

        string partitionKey = userId.ToString();
        string filter = $"PartitionKey eq '{partitionKey}' and Title eq '{subscriptionName}'";

        AsyncPageable<TableEntity> queryResults = tableClient.QueryAsync<TableEntity>(filter: filter, maxPerPage: 1, cancellationToken: cancellationToken);

        await foreach (var _ in queryResults)
        {
            return true;
        }

        return false; 
    }

    private async Task<TableClient> GetTableClient(CancellationToken cancellationToken = default)
    {
        var serviceClient = new TableServiceClient("");

        var tableClient = serviceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync(cancellationToken);
        return tableClient;
    }

    private Subscription ConvertToSubscription(TableEntity entity)
    {
        var subscription = new Subscription
        {
            Title = entity.GetString(nameof(Subscription.Title)),
            UserId = Convert.ToUInt32(entity.PartitionKey),
            Specialty = entity.GetString(nameof(Subscription.Specialty)),
            Experience = entity.GetDouble(nameof(Subscription.Experience)) ?? 0,
            PreferredWebsites = entity.GetString(nameof(Subscription.PreferredWebsites)),
            PartitionKey = entity.PartitionKey,
            RowKey = entity.RowKey,
            Timestamp = entity.Timestamp,
            ETag = entity.ETag
        };

        return subscription;
    }
}
