using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dispatcher.Application;
using MongoDB.Driver;

namespace Dispatcher.Infrastructure;

public class MongoAdminActionLogRepository : IAdminActionLogRepository
{
    private readonly IMongoCollection<AdminActionLog> _collection;

    public MongoAdminActionLogRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<AdminActionLog>("AdminActionLogs");
    }

    public async Task CreateAsync(AdminActionLog log)
    {
        await _collection.InsertOneAsync(log);
    }
}
