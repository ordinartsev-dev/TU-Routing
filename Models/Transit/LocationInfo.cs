using System.Text.Json.Serialization;
namespace Backend.Models
{
    public class LocationInfo
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; } // "location"
        [JsonPropertyName("Id")]
        public string Id { get; set; }
        [JsonPropertyName("Latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("Longitude")]
        public double Longitude { get; set; }
    }
}