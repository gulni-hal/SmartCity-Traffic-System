using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Traffic.Application.Entities;
using Traffic.Application.Interfaces;

namespace Traffic.Infrastructure.Repositories;

public class MongoTrafficRepository : ITrafficRepository
{
    private readonly IMongoCollection<TrafficRecord> _collection;

    public MongoTrafficRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<TrafficRecord>("TrafficRecords");
    }

    public async Task CreateAsync(TrafficRecord record)
    {
        await _collection.InsertOneAsync(record);
    }

    // YENİ EKLENEN METOT
    public async Task<IEnumerable<TrafficRecord>> GetByLocationIdAsync(string locationId)
    {
        // Lokasyona göre filtreleme yapıyoruz
        return await _collection.Find(t => t.LocationId == locationId).ToListAsync();
    }
    public async Task<IEnumerable<TrafficRecord>> GetHotspotsAsync()
    {
        return await _collection.Find(t => t.DensityLevel == "High").ToListAsync();
    }
}