using System.Text.Json.Serialization;
using Backend.Models;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Models
{
    public class RouteResponce
    {
        public string Type { get; set; }

        public List<double> Start { get; set; }

        public List<double> End { get; set; }

        public double DistanceMeters { get; set; }

        public int DurationSeconds { get; set; }

        public List<List<double>> Polyline { get; set; }

        public void SetPolylineFromDecodedPoints(IEnumerable<DecodedPoint> decodedPoints)
        {
            Polyline = decodedPoints.Select(point => new List<double> { point.Latitude, point.Longitude }).ToList();
        }
    }

    public class DecodedPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}