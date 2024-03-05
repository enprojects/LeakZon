using Leakzone.Backend.Accessors.DbModels;
using MongoDB.Driver;


namespace Leakzone.Backend.Infrastructure
{
    public interface IDbContext
    {
       IMongoCollection<SensorReadingDb> SensorReading { get; }
       Task DropSensorReadingCollectionAsync();
    }
}