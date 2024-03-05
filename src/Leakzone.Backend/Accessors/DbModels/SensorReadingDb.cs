using MongoDB.Bson.Serialization.Attributes;

namespace Leakzone.Backend.Accessors.DbModels
{
    public class SensorReadingDb
    {
        [BsonId]
        public int Id { get; set; }
        [BsonElement("SensorId")]
        public string SensorId { get; set; }
        [BsonElement("ReadingValue")]
        public double ReadingValue { get; set; }
        [BsonElement("Date")]
        //[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
    }
}
