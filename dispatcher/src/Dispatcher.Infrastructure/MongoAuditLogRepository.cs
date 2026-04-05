using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispatcher.Application;
using MongoDB.Driver;

namespace Dispatcher.Infrastructure;

public class MongoAuditLogRepository : IAuditLogRepository
{
    private readonly IMongoCollection<RequestAuditLog> _collection;

    public MongoAuditLogRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<RequestAuditLog>("RequestAuditLogs");
    }

    public async Task CreateAsync(RequestAuditLog log)
    {
        await _collection.InsertOneAsync(log);
    }
    public async Task<IEnumerable<RequestAuditLog>> GetRecentLogsAsync(int limit)
    {
        return await _collection.Find(_ => true)
                                .SortByDescending(x => x.CreatedAt)
                                .Limit(limit)
                                .ToListAsync();
    }
}
