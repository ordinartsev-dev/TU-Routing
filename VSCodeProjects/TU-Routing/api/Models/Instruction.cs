using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Instruction
    {
        [JsonPropertyName("distance")]
        public double distance { get; set; }
        [JsonPropertyName("heading")]
        public double heading { get; set; }
        [JsonPropertyName("sign")]
        public int sign { get; set; }
        [JsonPropertyName("interval")]
        public int interval [] { get; set; }
        [JsonPropertyName("text")]
        public string text { get; set; }
        [JsonPropertyName("time")]
        public int time { get; set; }
        [JsonPropertyName("street_name")]
        public string street_name { get; set; }
        [JsonPropertyName("last_heading")]
        public double last_heading { get; set; }

        public string PrintName()
        {
            return $"Stop Name: {Name ?? "[not specified]"}";
        }
    }
}