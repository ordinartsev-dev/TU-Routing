using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Hints
    {
        [JsonPropertyName("visited_nodes_sum")]
        public int visited_nodes_sum { get; set; }

        [JsonPropertyName("visited_nodes_average")]
        public int visited_nodes_average { get; set; }
    }
}
