using Leakzone.Backend.Managers.Models;

namespace Leakzone.Backend.Accessors.SensorAccessor
{
    public interface ISensorAccessor
    {
        Task InsertSensorsReading(IEnumerable<SensorInfo> sensors);

        Task<List<SensorInfo>> GetLatestSensorReadingsAsync(string sensorId = null);
        Task<List<SensorInfo>> GetOldestSensorReadingsAsync(string sensorId = null);
        Task<List<SensorInfo>> GetSensorsReadingsByRangeAsync(DateTime from, DateTime to, List<string> sensorIds = null);
    }
}