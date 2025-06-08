using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Info
    {
        [JsonPropertyName("copyrights")]
        public string [] copyrights { get; set; }
        [JsonPropertyName("took")]
        public int took { get; set; }
    }
}