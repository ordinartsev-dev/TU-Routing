using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class WalkingRoute
    {
        [JsonPropertyName("hints")]
        public Hints Hints { get; set; }
        [JsonPropertyName("info")]
        public Info Info{ get; set; }
        [JsonPropertyName("paths")]
        public Path[] Path{ get; set; }

        public string PrintLength()
        {
            return $"Walking route length: {Path.distance ?? "[not specified]"}";
        }
    }
}