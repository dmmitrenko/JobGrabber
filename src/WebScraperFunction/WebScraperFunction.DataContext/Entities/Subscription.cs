﻿using Azure;
using Azure.Data.Tables;

namespace WebScraperFunction.DataContext.Entities;
public class Subscription : ITableEntity
{
    public long ChatId { get; set; }
    public string Title { get; set; }
    public long UserId { get; set; }
    public string Specialty { get; set; }
    public double Experience { get; set; }
    public string PreferredWebsites { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
