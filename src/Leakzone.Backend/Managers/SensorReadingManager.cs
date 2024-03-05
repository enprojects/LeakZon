using Leakzone.Backend.Accessors.SensorAccessor;
using Leakzone.Backend.Managers.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using System.Globalization;

namespace Leakzone.Backend.Managers
{
    public class SensorReadingManager : ISensorReadingManager
    {
        private readonly ILogger<SensorReadingManager> _logger;
        private readonly ISensorAccessor _sensorAccessor;

        public SensorReadingManager(ILogger<SensorReadingManager> logger, ISensorAccessor sensorAccessor)
        {
            _logger = logger;
            _sensorAccessor = sensorAccessor;
        }

        public async Task InsertSensorsReadingAsync(List<JsonObject> rawSensorData)
        {
            // split the data into a batches
            var sensorsBatches = SplitRawSensorToBatches(rawSensorData);
            var insertToDbTasks = new List<Task>();

            foreach (var sensors in sensorsBatches)
            {
                insertToDbTasks.Add(_sensorAccessor.InsertSensorsReading(sensors));
            }
            //running number of task for the data insertion  
            await Task.WhenAll(insertToDbTasks);
        }

        public async Task<List<SensorInfo>> GetLatestReadingAsync(string sensorId)
        {
            var sensors = await _sensorAccessor.GetLatestSensorReadingsAsync(sensorId);
            return sensors;
        }
        public async Task<List<SensorInfo>> GetOldestReading(string sensorId)
        {
            var sensors = await _sensorAccessor.GetOldestSensorReadingsAsync(sensorId);
            return sensors;
        }

        public async Task<IEnumerable<SensorHourlyConsumption>> CalculateHourlyConsumptions(HourlyConsumptionRequest request)
        {

            var readings = await _sensorAccessor.GetSensorsReadingsByRangeAsync(request.From, request.To, request.SensorsIds);
            var result  = new List<SensorHourlyConsumption>();
            
            var groupedBySensor = readings.GroupBy(reading => reading.SensorId);

            foreach (var sensorGroup in groupedBySensor)
            {
                var hourlyGroups = sensorGroup
                    .GroupBy(reading => new DateTime(reading.Date.Year, reading.Date.Month, reading.Date.Day, reading.Date.Hour, 0, 0))
                    .OrderBy(group => group.Key);

                DateTime? previousHour = null;
                double previousReadingValue = 0;

                foreach (var hourGroup in hourlyGroups)
                {
                    if (previousHour != null)
                    {
                        var consumption = hourGroup.Last().ReadingValue - previousReadingValue;
                        result.Add(new SensorHourlyConsumption
                        {
                            SensorId = sensorGroup.Key,
                            Date = hourGroup.Key,
                            Consumption = consumption
                        });
                    }

                    previousHour = hourGroup.Key;
                    previousReadingValue = hourGroup.Last().ReadingValue;
                }
            }
            return result.OrderByDescending(c=>c.Consumption);
        }

        #region Private method
        private IList<IEnumerable<SensorInfo>> SplitRawSensorToBatches(List<JsonObject> rawSensorData)
        {
            var batchSize = 3000;
            var sensorBatches = new List<IEnumerable<SensorInfo>>();

            for (int i = 0; i < rawSensorData.Count; i += batchSize)
            {
                var batch = rawSensorData.Skip(i).Take(batchSize)
                    .Select(sensor =>
                    {
                        SensorInfo sensorDb;
                        try
                        {
                            sensorDb = ParseToSensorDb(sensor);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Sensor malformed json  {sensor?.ToString()}");
                            return null;
                        }

                        return sensorDb;
                    })
                    .ToList();

                sensorBatches.Add(batch.Where(x => x != null));
            }

            return sensorBatches;
        }

        private SensorInfo ParseToSensorDb(JsonObject jObjSensor)
        {
            var sensor = new SensorInfo();

            foreach (var property in jObjSensor)
            {
                switch (property.Key)
                {
                    case "Id":
                        sensor.Id = property.Value.GetValue<int>();
                        break;
                    case "SensorId":
                        sensor.SensorId = property.Value.ToString();
                        break;
                    case "ReadingValue":
                        sensor.ReadingValue = property.Value.GetValue<double>();
                        break;
                    case "Date":
                        sensor.Date = ParseDate(property.Value.ToString());
                        break;
                }
            }

            return sensor;
        }

        private DateTime ParseDate(string dateStr)
        {
            var formats = new[] { "dd/MM/yyyy H:mm", "dd/MM/yyyy HH:mm" };
            if (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime parsedDate))
            {
                return parsedDate;
            }
            throw new FormatException($"Unable to parse date: {dateStr}");
        }
        #endregion
    }
}
