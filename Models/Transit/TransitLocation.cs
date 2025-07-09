using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class TransitLocation
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("latitude")]
        public double latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double longitude { get; set; }
        [JsonPropertyName("is_stop")]
        public bool is_stop { get; set; }
    }
}