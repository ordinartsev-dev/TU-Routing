using System.Text.Json.Serialization;


namespace Backend.Models
{
    public class FeatureProperties
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; } // "stop" or null

        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }
        
        [JsonPropertyName("Location")]
        public LocationInfo Location { get; set; }
        
        [JsonPropertyName("Products")]
        public Dictionary<string, bool> Products { get; set; }
    }
}