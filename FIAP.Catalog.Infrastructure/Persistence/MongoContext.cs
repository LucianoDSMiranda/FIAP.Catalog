using FIAP.Catalog.Domain.Entities;
using MongoDB.Driver;

namespace FIAP.Catalog.Infrastructure.Persistence;

public class MongoSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
}

public class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IMongoClient client, MongoSettings settings)
    {
        _database = client.GetDatabase(settings.Database);
    }

    public IMongoCollection<Game> Games => _database.GetCollection<Game>("games");
    public IMongoCollection<UserGame> UserGames => _database.GetCollection<UserGame>("userGames");
}
