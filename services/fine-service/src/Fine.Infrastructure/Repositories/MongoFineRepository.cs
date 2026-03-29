using System.Threading.Tasks;
using MongoDB.Driver;
using Fine.Application.Entities;
using Fine.Application.Interfaces;

namespace Fine.Infrastructure.Repositories;

public class MongoFineRepository : IFineRepository
{
    private readonly IMongoCollection<FineRecord> _finesCollection;

    public MongoFineRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        // MongoDB'de "Fines" (Cezalar) adında bir tablo (collection) oluşturuyoruz
        _finesCollection = database.GetCollection<FineRecord>("Fines");
    }

    public async Task CreateAsync(FineRecord record)
    {
        await _finesCollection.InsertOneAsync(record);
    }
}