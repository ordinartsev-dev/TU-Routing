using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Path
    {
        [JsonPropertyName("distance")]
        public double distance { get; set; }
        [JsonPropertyName("weight")]
        public double weight { get; set; }
        [JsonPropertyName("time")]
        public int time { get; set; }
        [JsonPropertyName("transfers")]
        public int transfers { get; set; }
        [JsonPropertyName("points_encoded")]
        public bool points_encoded { get; set; }
        [JsonPropertyName("bbox")]
        public double[] bbox { get; set; }
        [JsonPropertyName("points")]
        public string points { get; set; }
        [JsonPropertyName("instructions")]
        public Instruction[] instructions{ get; set; }
        [JsonPropertyName("legs")]
        public List<object> legs { get; set; }
        [JsonPropertyName("details")]
        public Dictionary<string, object> details { get; set; }
        [JsonPropertyName("ascend")]
        public double ascend { get; set; }
        [JsonPropertyName("descend")]
        public double descend { get; set; }
        [JsonPropertyName("snapped_waypoints")]
        public string snapped_waypoints { get; set; }
    }
}