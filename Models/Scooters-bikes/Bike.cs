using System.Text.Json.Serialization;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Bike
    {
        [JsonPropertyName("provider")]
        public string provider { get; set; }
        [JsonPropertyName("lat")]
        public double lat { get; set; }
        [JsonPropertyName("lon")]
        public double lon { get; set; }
        [JsonPropertyName("vehicle_id")]
        public string vehicle_id { get; set; }
        [JsonPropertyName("distance")]
        public double distance { get; set; }
    }
}