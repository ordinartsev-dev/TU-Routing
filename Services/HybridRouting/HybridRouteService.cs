using System.Net.Http;
using System.Threading.Tasks;
using Backend.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Services
{
    public class HybridRouteService
    {
        private readonly GraphHopperService _graphHopperService;
        private readonly TransitRouteService _transitRouteService;
        private readonly FindTheNearestStationService _findTheNearestStationService;

        public HybridRouteService(GraphHopperService graphHopperService, TransitRouteService transitRouteService,
            FindTheNearestStationService findTheNearestStationService)
        {
            _graphHopperService = graphHopperService;
            _transitRouteService = transitRouteService;
            _findTheNearestStationService = findTheNearestStationService;
        }

        public async Task<(WalkingRoute obj, string jsonResponse)> generateWalkingRoute(double fromLat, double fromLon,
            double toLat, double toLon)
        {
            Console.WriteLine($"Requesting route from {fromLat},{fromLon} to {toLat},{toLon}");
            var (obj, jsonResponse) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);
            return (obj, jsonResponse);
        }

        public async Task<TransitRoute> generateTransitRoute(double fromLat, double fromLon, double toLat, double toLon, string time)
        {
            return await _transitRouteService.CalculateTransitRouteAsync(fromLat, fromLon, toLat, toLon, time);
        }

        public async Task<string> generateHybridRoute(double fromLat, double fromLon, double toLat, double toLon)
        {
            DateTime currentTime = DateTime.Now;
            List<TransitRouteResponse> transitRouteResponses = new();

            var trRoute = await generateTransitRoute(fromLat, fromLon, toLat, toLon,
                currentTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            if (trRoute == null || trRoute.routes.Length == 0)
                return "No valid transit route found.";

            var firstStation = trRoute.routes[0].legs[0].end;
            if (firstStation == null)
                return "No valid first station found in the transit route.";

            var lastStation = trRoute.routes[0].legs[^1].start;
            if (lastStation == null)
                return "No valid last station found in the transit route.";

            var (walkingRouteToFirstStation, jsonResponseToFirstStation) = await generateWalkingRoute(fromLat, fromLon,
                firstStation.latitude, firstStation.longitude);
            if (walkingRouteToFirstStation?.Path == null || walkingRouteToFirstStation.Path.Length == 0)
                return "Invalid GraphHopper walking route to first station.";

            var (walkingRouteToLastStation, jsonResponseToLastStation) = await generateWalkingRoute(
                lastStation.latitude, lastStation.longitude, toLat, toLon);
            if (walkingRouteToLastStation?.Path == null || walkingRouteToLastStation.Path.Length == 0)
                return "Invalid GraphHopper walking route to last station.";

            var firstLeg = trRoute.routes[0].legs[0];
            if (firstLeg.arrival_time == null || firstLeg.departure_time == null)
                return "Invalid time data for walking segment to the first station.";
            var bvgWalkingTimeToFirstStation = (int)(firstLeg.arrival_time - firstLeg.departure_time).TotalSeconds;

            var lastLeg = trRoute.routes[0].legs[^1];
            if (lastLeg.arrival_time == null || lastLeg.departure_time == null)
                return "Invalid time data for walking segment from the last station.";
            var bvgWalkingTimeToLastStation = (int)(lastLeg.arrival_time - lastLeg.departure_time).TotalSeconds;

            int graphhopperWalkingTimeToFirstStation = walkingRouteToFirstStation.Path[0].time;
            int graphhopperWalkingTimeToLastStation = walkingRouteToLastStation.Path[0].time;
            
            Console.WriteLine("Time: " + graphhopperWalkingTimeToFirstStation);

            var decodedPoints1 = PolylineDecoder.Decode(walkingRouteToFirstStation.Path[0].points);
            if (decodedPoints1 == null || decodedPoints1.Count == 0)
                Console.WriteLine("Invalid decoded points for walking route to first station.");
            var decodedPoints2 = PolylineDecoder.Decode(walkingRouteToLastStation.Path[0].points);
            if (decodedPoints2 == null || decodedPoints2.Count == 0)
                Console.WriteLine("Invalid decoded points for walking route to last station.");

            var walkToDistance = walkingRouteToFirstStation.Path[0].distance;
            var walkFromDistance = walkingRouteToLastStation.Path[0].distance;

            var walkingPolylineBVGtoFirstStation = new List<List<double>>();
            foreach (var feature in trRoute.routes[0].legs[0].polyline?.features ?? new List<Feature>())
            {
                if (feature.geometry?.Coordinates?.Count >= 2)
                {
                    walkingPolylineBVGtoFirstStation.Add(new List<double>
                    {
                        feature.geometry.Coordinates[1],
                        feature.geometry.Coordinates[0]
                    });
                }
            }

            var walkingPolylineBVGfromLastStation = new List<List<double>>();
            foreach (var feature in trRoute.routes[0].legs[^1].polyline?.features ?? new List<Feature>())
            {
                if (feature.geometry?.Coordinates?.Count >= 2)
                {
                    walkingPolylineBVGfromLastStation.Add(new List<double>
                    {
                        feature.geometry.Coordinates[1],
                        feature.geometry.Coordinates[0]
                    });
                }
            }

            var segments = new List<HybridRouteSegment>();
            
            segments.Add(new HybridRouteSegment
            {
                Type = "walk",
                Polyline = decodedPoints1.Select(p => new List<double> { p.Latitude, p.Longitude }).ToList(),
                DurationSeconds = graphhopperWalkingTimeToFirstStation /1000,
                DistanceMeters = (int)walkToDistance
            });
            

            var legs = trRoute.routes[0].legs;

            for (int i = 1; i < legs.Length - 1; i++)
            {
                var leg = legs[i];

                var stopoverPolyline = leg.stopovers?
                    .Select(stop => new List<double> { stop.latitude, stop.longitude })
                    .ToList() ?? new List<List<double>>();

                var precisePolyline = new List<List<double>>();
                foreach (var feature in leg.polyline?.features ?? new List<Feature>())
                {
                    if (feature.geometry?.Coordinates?.Count >= 2)
                    {
                        precisePolyline.Add(new List<double>
                        {
                            feature.geometry.Coordinates[1],
                            feature.geometry.Coordinates[0]
                        });
                    }
                }

                if (leg.departure_time == null || leg.arrival_time == null)
                    return "Invalid time in transit leg.";

                var segment = new HybridRouteSegment
                {
                    Type = "transit",
                    DepartureTime = leg.departure_time.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ArrivalTime = leg.arrival_time.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Polyline = stopoverPolyline,
                    precisePolyline = new List<List<List<double>>> { precisePolyline },
                    DurationSeconds = (int)(leg.arrival_time.ToUnixTimeSeconds() - leg.departure_time.ToUnixTimeSeconds()),
                    DistanceMeters = leg.distance ?? 0,
                    TransportType = leg.type ?? "Unknown",
                    TransportLine = string.IsNullOrEmpty(leg.line) ? "Unknown" : leg.line,
                    FromStop = leg.start?.name ?? "Unknown",
                    ToStop = leg.end?.name ?? "Unknown"
                };

                // Set segment type and transport info
                if (leg.type == "walking")
                {
                    segment.Type = "walk";
                }
                else
                {
                    segment.Type = "transit";
                    segment.TransportType = leg.type ?? "Unknown";
                    segment.TransportLine = string.IsNullOrEmpty(leg.line) ? "Unknown" : leg.line;
                }

                segments.Add(segment);
            }
            
            segments.Add(new HybridRouteSegment
            {
                Type = "walk",
                Polyline = decodedPoints2.Select(p => new List<double> { p.Latitude, p.Longitude }).ToList(),
                DurationSeconds = graphhopperWalkingTimeToLastStation / 1000,
                DistanceMeters = (int)walkFromDistance
            });

            if (segments.Count == 0)
                return "No valid hybrid route found.";

            var response = new TransitRouteResponse
            {
                Type = "Hybrid",
                Start = new List<double> { fromLat, fromLon },
                End = new List<double> { toLat, toLon },
                Segments = segments,
                DurationSeconds = segments.Sum(s => s.DurationSeconds),
                DistanceMeters = segments.Sum(s => s.DistanceMeters)
            };

            return JsonSerializer.Serialize(response);
        }
    }
}
