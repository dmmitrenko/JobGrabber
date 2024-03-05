using Azure;
using Azure.Data.Tables;
using UserManagementFunction.Domain.Enums;

namespace UserManagementFunction.DataContext.Entities;
public class Subscription : ITableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Keyword { get; set; }
    public double Experience { get; set; }
    public string EnglishLevel { get; set; }
    public string PreferredWebsites { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
