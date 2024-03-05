using Leakzone.Backend.Managers.Models;
using System.Text.Json.Nodes;

namespace Leakzone.Backend.Managers
{
    public interface ISensorReadingManager
    {
        Task InsertSensorsReadingAsync(List<JsonObject> sensors);

        Task<List<SensorInfo>> GetLatestReadingAsync(string sensorId);
        Task<List<SensorInfo>> GetOldestReading(string sensorId);
        Task<IEnumerable<SensorHourlyConsumption>> CalculateHourlyConsumptions(HourlyConsumptionRequest request);
    }
}