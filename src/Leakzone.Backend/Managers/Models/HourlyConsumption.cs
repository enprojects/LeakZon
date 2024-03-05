namespace Leakzone.Backend.Managers.Models
{
    public class SensorHourlyConsumption
    {
        public string SensorId { get; set; }
        public double Consumption { get; set; }
        public DateTime Date { get; set; }
    }
}