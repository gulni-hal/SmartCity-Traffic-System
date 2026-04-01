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

    // YENİ EKLENEN METOT
    public async Task<IEnumerable<FineRecord>> GetByLicensePlateAsync(string licensePlate)
    {
        // Plakaya göre filtreleme yapıyoruz
        return await _finesCollection.Find(f => f.LicensePlate == licensePlate).ToListAsync();
    }
}