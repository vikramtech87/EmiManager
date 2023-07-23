using MongoDB.Driver;

namespace EmiManager.Api.Data;

public class MongoDbContext {
    private readonly MongoClient _client;

    public IMongoDatabase Db => _client.GetDatabase("EmiManager");

    public MongoDbContext(string connStr)
    {
        var settings = MongoClientSettings.FromConnectionString(connStr);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        _client = new MongoClient(settings);
    }
}
