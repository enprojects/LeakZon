
namespace Leakzone.Backend.Managers.Models
{
    public class SensorInfo
    {
        public int Id { get; set; }
        public string SensorId { get; set; }
        public double ReadingValue { get; set; }
        public DateTime Date { get; set; }
    }
}