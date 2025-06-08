using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class TransitRoute
    {
        [JsonPropertyName("routes")]
        public TransitRouteDetail[] routes { get; set; }
    }
}