using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class TransitRouteDetail
    {
        [JsonPropertyName("legs")]
        public Leg[] legs { get; set; }
        [JsonPropertyName("duration_minutes")]
        public int duration_minutes { get; set; }
        [JsonPropertyName("transfers")]
        public int transfers { get; set; }
        [JsonPropertyName("walking_distance")]
        public int walking_distance { get; set; }
        [JsonPropertyName("departure_time")]
        public DateTimeOffset departure_time { get; set; }
        [JsonPropertyName("arrival_time")]
        public DateTimeOffset arrival_time { get; set; }

        public string PrintRouteDetails()
        {
            return $"Transit Route Detail: {duration_minutes} minutes, {transfers} transfers, walking distance {walking_distance} meters";
        }
    }

}