using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Leg
    {
        [JsonPropertyName("start")]
        public TransitLocation start { get; set; }
        [JsonPropertyName("end")]
        public TransitLocation end { get; set; }
        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonPropertyName("line")]
        public string? line { get; set; }
        [JsonPropertyName("direction")]
        public string? direction { get; set; }
        [JsonPropertyName("departure_time")]
        public DateTimeOffset departure_time { get; set; }
        [JsonPropertyName("arrival_time")]
        public DateTimeOffset arrival_time { get; set; }
        [JsonPropertyName("delay_minutes")]
        public int? delay_minutes { get; set; }
        [JsonPropertyName("distance")]
        public int? distance { get; set; }
        [JsonPropertyName("platform")]
        public string? platform { get; set; }
        [JsonPropertyName("warnings")]
        public string[] warnings { get; set; }
        [JsonPropertyName("stopovers")]
        public Stopover[] stopovers { get; set; }
        [JsonPropertyName("polyline")]
        public Polyline polyline { get; set; } // New field
    }
}