using System.Text.Json.Serialization;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Stopover
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("latitude")]
        public double latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double longitude { get; set; }
        [JsonPropertyName("arival_time")]
        public string arival_time { get; set; }
        [JsonPropertyName("departure_time")]
        public string departure_time { get; set; }
        [JsonPropertyName("platform")]
        public string? platform { get; set; }
    }
}