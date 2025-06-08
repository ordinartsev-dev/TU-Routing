namespace Backend.Models;
public class HybridRouteSegment
{
    public string Type { get; set; } // "walk" or "transit"
    public List<List<double>> Polyline { get; set; }

    public List<List<double>> precisePolyline { get; set; }
    public int DurationSeconds { get; set; }
    public int DistanceMeters { get; set; }
    public string? TransportType { get; set; }
    public string? TransportLine { get; set; }
    public string? FromStop { get; set; }
    public string? ToStop { get; set; }
}