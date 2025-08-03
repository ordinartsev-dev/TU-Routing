using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Backend.Models
{
    public class GeometryCustom
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; } // "CustomPoint" or others
        [JsonPropertyName("Coordinates")]
        public List<double> Coordinates { get; set; } // [longitude, latitude]
    }
}