using System.Text.Json.Serialization;


namespace Backend.Models
{
    public class Polyline
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; } // "FeatureCollection"

        [JsonPropertyName("features")]
        public List<Feature> features { get; set; }
    }
}