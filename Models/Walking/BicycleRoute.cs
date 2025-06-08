using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class BicycleRoute
    {
        [JsonPropertyName("hints")]
        public Hints Hints { get; set; }
        [JsonPropertyName("info")]
        public Info Info{ get; set; }
        [JsonPropertyName("paths")]
        public Path[] Path{ get; set; }

        public string PrintLength()
        {
            if (Path == null || Path.Length == 0)
            {
                return "Cycling route length: [not specified]";
            }

            return $"Cycling route length: {Path[0].distance}";
        }
    }
}