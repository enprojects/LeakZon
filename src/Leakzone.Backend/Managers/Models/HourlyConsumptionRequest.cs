namespace Leakzone.Backend.Managers.Models
{
    public class HourlyConsumptionRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<string> SensorsIds { get; set; }
    }
}
