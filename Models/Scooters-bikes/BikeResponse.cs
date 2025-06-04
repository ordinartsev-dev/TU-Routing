using System.Text.Json.Serialization;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.Models
{
    public class BikeResponse
    {
        [JsonPropertyName("bikes")]
        public Bike[] bike { get; set; }
    }
}