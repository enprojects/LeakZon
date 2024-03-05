namespace Leakzon.WebApi.ViewModels
{
    public class SensorInfoViewModel
    {
        public int Id { get; set; }
        public string SensorId { get; set; }
        public double ReadingValue { get; set; }
        public DateTime Date { get; set; }
    }
}
