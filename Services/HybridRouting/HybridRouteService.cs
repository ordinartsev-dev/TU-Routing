// Services/TransitRouteService.cs
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

        public async Task<PublicTransportStop[]> findTheNearestStation(double Lat, double Lon)
        {
            // Implement the logic to find the nearest station
            // This could involve calling other services or APIs
            // For example, you might call the FindTheNearestStationService here
            var nearestStation = await _findTheNearestStationService.FindTheNearestStationServiceAsync(Lat, Lon);
            return nearestStation;
        }

        //public async Task<WalkingRoute> generateRouteToStation(double fromLat, double fromLon, double toLat, double toLon)
        public async Task<(WalkingRoute obj, string jsonResponse)> generateWalkingRoute(double fromLat, double fromLon,
            double toLat, double toLon)
        {
            // Implement the logic to generate a route to the station
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            Console.WriteLine($"Requesting route from {fromLat},{fromLon} to {toLat},{toLon}");
            var (obj, jsonResponse) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);

            return (obj, jsonResponse);
        }

        public async Task<TransitRoute> generateTransitRoute(PublicTransportStop stop1, PublicTransportStop stop2,
                string depatureTime)
            //public async Task<string> generateTransitRoute(PublicTransportStop stop1, PublicTransportStop stop2)
        {

            var part2 = await _transitRouteService.CalculateTransitRouteAsync(stop1.Latitude, stop1.Longitude,
                stop2.Latitude, stop2.Longitude, depatureTime);

            return part2;
        }


        public async Task<string> generateHybridRoute(double fromLat, double fromLon, double toLat, double toLon)
        {
            var nearestStations1 = await findTheNearestStation(fromLat, fromLon);
            if (nearestStations1 == null || nearestStations1.Length == 0)
            {
                return "No nearest station found from the starting point.";
            }

            var nearestStations2 = await findTheNearestStation(toLat, toLon);
            if (nearestStations2 == null || nearestStations2.Length == 0)
            {
                return "No nearest station found from the endpoint.";
            }

            DateTime currentTime = DateTime.Now;
            List<TransitRouteResponse> transitRouteResponses = new();

            foreach (var station1 in nearestStations1)
            {
                foreach (var station2 in nearestStations2)
                {
                    var (walkingRoute1, _) =
                        await generateWalkingRoute(fromLat, fromLon, station1.Latitude, station1.Longitude);
                    if (walkingRoute1?.Path == null || walkingRoute1.Path.Length == 0 ||
                        string.IsNullOrEmpty(walkingRoute1.Path[0].points))
                        continue;

                    int walkingTime1 = walkingRoute1.Path[0].time;
                    DateTime departureTime = currentTime.AddMilliseconds(walkingTime1);

                    var trRoute = await generateTransitRoute(station1, station2,
                        depatureTime: departureTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    if (trRoute?.routes == null || trRoute.routes.Length == 0)
                        continue;

                    var lastRoute = trRoute.routes[0];
                    var lastLeg = lastRoute.legs[lastRoute.legs.Length - 2];
                    var lastStopover = lastLeg.stopovers[lastLeg.stopovers.Length - 1];

                    WalkingRoute walkingRoute2;
                    if (lastStopover.latitude == station2.Latitude && lastStopover.longitude == station2.Longitude)
                    {
                        var result = await generateWalkingRoute(station2.Latitude, station2.Longitude, toLat, toLon);
                        walkingRoute2 = result.obj;
                        if (walkingRoute2?.Path == null || walkingRoute2.Path.Length == 0 ||
                            string.IsNullOrEmpty(walkingRoute2.Path[0].points))
                            continue;
                    }
                    else
                    {
                        var result = await generateWalkingRoute(lastStopover.latitude, lastStopover.longitude, toLat,
                            toLon);
                        walkingRoute2 = result.obj;
                        if (walkingRoute2?.Path == null || walkingRoute2.Path.Length == 0 ||
                            string.IsNullOrEmpty(walkingRoute2.Path[0].points))
                            continue;
                    }

                    var decodedPoints1 = PolylineDecoder.Decode(walkingRoute1.Path[0].points);
                    var decodedPoints2 = PolylineDecoder.Decode(walkingRoute2.Path[0].points);

                    var walkToDistance = walkingRoute1.Path[0].distance;
                    var walkFromDistance = walkingRoute2.Path[0].distance;

                    var segments = new List<HybridRouteSegment>();

                    // Add walking segment TO station
                    segments.Add(new HybridRouteSegment
                    {
                        Type = "walk",
                        Polyline = decodedPoints1.Select(p => new List<double> { p.Latitude, p.Longitude }).ToList(),
                        DurationSeconds = walkingRoute1.Path[0].time / 1000,
                        DistanceMeters = (int)walkToDistance
                    });

                    // Add each transit leg as a separate segment
                    foreach (var leg in lastRoute.legs)
                    {
                        if (leg.type == "walking") continue;

                        var stopoverPolyline = leg.stopovers?
                            .Select(stop => new List<double> { stop.latitude, stop.longitude })
                            .ToList() ?? new List<List<double>>();

                        var precisePolyline = new List<List<double>>();
                        if (leg.polyline?.features != null)
                        {
                            foreach (var feature in leg.polyline.features)
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
                        }

                        if (precisePolyline.Count == 0 && stopoverPolyline.Count == 0)
                            continue;

                        segments.Add(new HybridRouteSegment
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
                            FromStop = leg.start.name ?? "Unknown",
                            ToStop = leg.end.name ?? "Unknown"
                        });
                    }

                    // Add walking segment FROM station
                    segments.Add(new HybridRouteSegment
                    {
                        Type = "walk",
                        Polyline = decodedPoints2.Select(p => new List<double> { p.Latitude, p.Longitude }).ToList(),
                        DurationSeconds = walkingRoute2.Path[0].time / 1000,
                        DistanceMeters = (int)walkFromDistance
                    });

                    int totalDuration = segments.Sum(s => s.DurationSeconds);
                    int totalDistance = segments.Sum(s => s.DistanceMeters);

                    var response = new TransitRouteResponse
                    {
                        Type = "Hybrid",
                        Start = new List<double> { decodedPoints1[0].Latitude, decodedPoints1[0].Longitude },
                        End = new List<double> { decodedPoints2[^1].Latitude, decodedPoints2[^1].Longitude },
                        Segments = segments,
                        DurationSeconds = totalDuration,
                        DistanceMeters = totalDistance
                    };

                    transitRouteResponses.Add(response);
                }
            }

            var sortedResponses = transitRouteResponses.OrderBy(r => r.DurationSeconds).ToList();

            if (sortedResponses.Count == 0)
            {
                return "No valid hybrid route found.";
            }

            return JsonSerializer.Serialize(sortedResponses[0]);
        }
    }
}
