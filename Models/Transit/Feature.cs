using System.Text.Json.Serialization;


namespace Backend.Models
{
    public class Feature
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; } // "Feature"
        [JsonPropertyName("Properties")]
        public FeatureProperties Properties { get; set; }
        [JsonPropertyName("Geometry")]
        public Geometry Geometry { get; set; }
    }
}