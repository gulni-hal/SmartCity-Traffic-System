using System.Collections.Generic;
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
        _finesCollection = database.GetCollection<FineRecord>("Fines");
    }

    public async Task CreateAsync(FineRecord record)
    {
        await _finesCollection.InsertOneAsync(record);
    }

    public async Task<IEnumerable<FineRecord>> GetByLicensePlateAsync(string licensePlate)
    {
        return await _finesCollection.Find(f => f.LicensePlate == licensePlate).ToListAsync();
    }

    public async Task<IEnumerable<FineRecord>> GetAllAsync()
    {
        return await _finesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _finesCollection.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }
}
