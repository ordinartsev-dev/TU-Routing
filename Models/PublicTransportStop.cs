using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class PublicTransportStop
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("products")]
        public Dictionary<string, bool> Products { get; set; }

        [JsonPropertyName("distance")]
        public int Distance { get; set; }

        [JsonPropertyName("station")]
        public object Station { get; set; } // Adjust type if you have a Station class

        [JsonPropertyName("lines")]
        public object[] Lines { get; set; } // Adjust type if you have a Line class

        public string PrintName()
        {
            return $"Stop Name: {Name ?? "[not specified]"}";
        }
    }
}