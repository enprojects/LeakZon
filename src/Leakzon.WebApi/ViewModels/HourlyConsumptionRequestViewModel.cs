namespace Leakzon.WebApi.ViewModels
{
    public class HourlyConsumptionRequestViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<string>? SensorsIds { get; set; }
    }
}
