using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Models
{
    public class TransitRouteResponse
    {
        public string Type { get; set; }

        public List<double> Start { get; set; }     // начальная точка маршрута
        public List<double> End { get; set; }       // конечная точка маршрута

        public int DistanceMeters { get; set; }
        public int DurationSeconds { get; set; }

        // Новый список сегментов маршрута
        public List<HybridRouteSegment> Segments { get; set; }

        /*
        public TransitRouteResponse(string RouteId, List<double> Start, List<double> End, int DistanceMeters, int DurationSeconds,
         List<List<double>> WalkToTransportPolyline, List<List<double>> TransportPolyline, List<List<double>> WalkFromTransportPolyline,
         string TransportType, string TransportLine, string FromStop, string ToStop)
        {
            this.RouteId = RouteId;
            this.Start = Start;
            this.End = End;
            this.DistanceMeters = DistanceMeters;
            this.DurationSeconds = DurationSeconds;
            this.WalkToTransportPolyline = WalkToTransportPolyline;
            this.TransportPolyline = TransportPolyline;
            this.WalkFromTransportPolyline = WalkFromTransportPolyline;
            this.TransportType = TransportType;
            this.TransportLine = TransportLine;
            this.FromStop = FromStop;
            this.ToStop = ToStop;
        }
        */
    }
}