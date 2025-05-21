using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Hints
    {
        [JsonPropertyName("type")]
        public int visited_nodes_sum { get; set; }
        [JsonPropertyName("id")]
        public int visited_nodes_average { get; set; }

        public string PrintName()
        {
            return $"Stop Name: {Name ?? "[not specified]"}";
        }
    }

        public string PrintName()
        {
            return $"Stop Name: {Name ?? "[not specified]"}";
        }
    }
}