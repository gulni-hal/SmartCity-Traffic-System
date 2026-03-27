using System.Threading.Tasks;
using MongoDB.Driver;
using Auth.Application.Entities;
using Auth.Application.Interfaces;

namespace Auth.Infrastructure.Repositories;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _usersCollection;

    public MongoUserRepository(string connectionString, string databaseName)
    {
        //MongoDB'ye baglanip "Users" isimli tabloyu (koleksiyonu) seciyoruz
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _usersCollection = database.GetCollection<User>("Users");
    }

    public async Task CreateAsync(User user)
    {
        await _usersCollection.InsertOneAsync(user);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        //Veritabaninda bu kullanici adiyla eslesen ilk kaydi getirir
        return await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
    }
}