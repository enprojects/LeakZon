using AutoMapper;
using Leakzone.Backend.Accessors.DbModels;
using Leakzone.Backend.Infrastructure;
using Leakzone.Backend.Managers.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Leakzone.Backend.Accessors.SensorAccessor
{
    public class SensorAccessor : ISensorAccessor
    {
        private readonly ILogger<SensorAccessor> _logger;
        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SensorAccessor(ILogger<SensorAccessor> logger,
                              IDbContext context,
                              IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task InsertSensorsReading(IEnumerable<SensorInfo> sensors)
        {
            var bulk = new List<WriteModel<SensorReadingDb>>();

            foreach (var reading in sensors)
            {
                var sensorReading = _mapper.Map<SensorReadingDb>(reading);

                var filter = Builders<SensorReadingDb>.Filter.Eq(r => r.Id, sensorReading.Id);
                var update = Builders<SensorReadingDb>.Update
                    .Set(r => r.SensorId, sensorReading.SensorId)
                    .Set(r => r.ReadingValue, sensorReading.ReadingValue)
                    .Set(r => r.Date, sensorReading.Date);
                var upsertOne = new UpdateOneModel<SensorReadingDb>(filter, update) { IsUpsert = true };
                bulk.Add(upsertOne);
            }
            await _context.SensorReading.BulkWriteAsync(bulk);
        }

        public async Task<List<SensorInfo>> GetLatestSensorReadingsAsync(string sensorId = null)
        {
            List<SensorReadingDb> result;
            if (string.IsNullOrEmpty(sensorId))
            {
                // Get the latest reading for every sensor
                var pipeline = new BsonDocument[]
                {
                    new ("$sort", new BsonDocument("Date", -1)), //-1 is for desc
                    new ("$group", new BsonDocument
                    {
                        { "_id", "$SensorId" },
                        { "latestReading", new BsonDocument("$first", "$$ROOT") }
                    }),
                    new ("$replaceRoot", new BsonDocument("newRoot", "$latestReading"))
                };

                result = await _context.SensorReading.Aggregate<SensorReadingDb>(pipeline).ToListAsync();
                return _mapper.Map<List<SensorInfo>>(result);
            }

            // Get the latest reading for the specified sensor
            var filter = Builders<SensorReadingDb>.Filter.Eq("SensorId", sensorId);

            // Use Find with chaining for SortByDescending and Limit
            result = await _context.SensorReading.Find(filter)
                .SortByDescending(sr => sr.Date)
                .Limit(1)
                .ToListAsync();

            return _mapper.Map<List<SensorInfo>>(result);
        }

        public async Task<List<SensorInfo>> GetOldestSensorReadingsAsync(string sensorId = null)
        {
            List<SensorReadingDb> result;
            var pipeline = new List<BsonDocument>();

            if (!string.IsNullOrEmpty(sensorId))
            {
                pipeline.Add(new BsonDocument("$match", new BsonDocument("SensorId", sensorId)));
            }

            pipeline.AddRange(new[]
            {
                new BsonDocument("$sort", new BsonDocument { { "SensorId", 1 }, { "Date", 1 } }), // 1 is for ascending
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$SensorId" },
                    { "oldestReading", new BsonDocument("$first", "$$ROOT") }
                }),
                new BsonDocument("$replaceRoot", new BsonDocument("newRoot", "$oldestReading"))
            });

             result = await _context.SensorReading
                .Aggregate<SensorReadingDb>(pipeline)
                .ToListAsync();

             return _mapper.Map<List<SensorInfo>>(result);
        }

        public async Task<List<SensorInfo>> GetSensorsReadingsByRangeAsync(DateTime from, DateTime to, List<string> sensorIds = null)
        {
            var filterBuilder = Builders<SensorReadingDb>.Filter;
            var dateFilter = filterBuilder.Gte(reading => reading.Date, from) & filterBuilder.Lte(reading => reading.Date, to);
            var sensorIdFilter = sensorIds != null && sensorIds.Count > 0 ? filterBuilder.In(reading => reading.SensorId, sensorIds) : null;

            var filter = sensorIdFilter != null ? filterBuilder.And(dateFilter, sensorIdFilter) : dateFilter;

            var result  = await _context.SensorReading
                .Find(filter)
                .SortBy(reading => reading.SensorId)
                .ThenBy(reading => reading.Date)
                .ToListAsync();

            return _mapper.Map<List<SensorInfo>>(result);
        }
    }
}
