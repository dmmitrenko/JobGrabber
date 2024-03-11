using AutoMapper;
using Azure.Data.Tables;
using UserManagementFunction.DataContext.Entities;
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

    public async Task<List<Domain.Models.Subscription>> GetAllSubscriptionsByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        //var tableClient = await GetTableClient(cancellationToken);

        //string filter = $"PartitionKey eq '{userId}'";


        //List<Subscription> subscriptions = new List<Subscription>();

        //await foreach (var entity in tableClient.QueryAsync<TableEntity>(filter: filter, cancellationToken: cancellationToken))
        //{
        //    var subscription = new Subscription
        //    {
        //        Id = entity["Id"] != null ? Guid.Parse(entity.GetString("Id")) : Guid.Empty,
        //        UserId = Guid.Parse(entity.PartitionKey),
        //        Keyword = entity.GetString("Keyword"),
        //        Experience = entity.GetDouble("Experience") ?? 0,
        //        EnglishLevel = entity.GetString("EnglishLevel"),
        //        PreferredWebsites = entity.GetString("PreferredWebsites"),
        //        PartitionKey = entity.PartitionKey,
        //        RowKey = entity.RowKey,
        //        Timestamp = entity.Timestamp,
        //        ETag = entity.ETag
        //    };
        //    subscriptions.Add(subscription);
        //}

        //return _mapper.Map<List<Domain.Models.Subscription>>(subscriptions);
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.Subscription> GetSubscriptionById(Guid userId, Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        var tableClient = await GetTableClient(cancellationToken);
        var subscription = await tableClient.GetEntityAsync<Subscription>(userId.ToString(), subscriptionId.ToString());

        return _mapper.Map<Domain.Models.Subscription>(subscription);

    }

    private async Task<TableClient> GetTableClient(CancellationToken cancellationToken = default)
    {
        var serviceClient = new TableServiceClient("");

        var tableClient = serviceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync(cancellationToken);
        return tableClient;
    }
}
