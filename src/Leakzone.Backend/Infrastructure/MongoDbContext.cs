using Leakzone.Backend.Accessors.DbModels;
using Leakzone.Backend.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Leakzone.Backend.Infrastructure
{
    public class MongoDbContext : IDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(ILogger<MongoDbContext> logger, DbConfiguration configuration)
        {
            var client = new MongoClient(configuration.Connection);
            _database = client.GetDatabase(configuration.DbName);
        }

        public async Task DropSensorReadingCollectionAsync()
        {
            await _database.DropCollectionAsync("SensorReading");
        }

        public IMongoCollection<SensorReadingDb> SensorReading => _database.GetCollection<SensorReadingDb>("SensorReading");
    }
}
