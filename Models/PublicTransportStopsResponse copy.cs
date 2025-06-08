using System.Text.Json.Serialization;
using System.Collections.Generic;


namespace Backend.Models
{
    public class PublicTransportStopsResponse
    {
        [JsonPropertyName("stops")]
        public PublicTransportStop[] stops { get; set; }

        [JsonPropertyName("message")]
        public string message { get; set; }
        
    }
}