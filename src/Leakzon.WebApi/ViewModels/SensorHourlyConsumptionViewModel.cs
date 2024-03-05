namespace Leakzon.WebApi.ViewModels
{
    public class SensorHourlyConsumptionViewModel
    {
        public string SensorId { get; set; }
        public double Consumption { get; set; }
        public DateTime Date { get; set; }
    }
}
