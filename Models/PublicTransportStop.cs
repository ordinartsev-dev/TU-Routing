using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class PublicTransportStop
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("id")]
        public String Id {get; set;}
        [JsonPropertyName("name")]
        public String Name {get; set;}
        [JsonPropertyName("location")]
        public Location Location {get; set;}
        [JsonPropertyName("products")]
        public Dictionary<string, bool> Products { get; set; }
        [JsonPropertyName("distance")]
        public int Distance { get; set; }

        public string PrintName()
        {
            return $"Stop Name: {Name ?? "[not specified]"}";
        }
    }
}