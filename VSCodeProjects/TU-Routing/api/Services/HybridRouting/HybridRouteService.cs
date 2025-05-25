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

        public HybridRouteService(GraphHopperService graphHopperService, TransitRouteService transitRouteService, FindTheNearestStationService findTheNearestStationService)
        {
            _graphHopperService = graphHopperService;
            _transitRouteService = transitRouteService;
            _findTheNearestStationService = findTheNearestStationService;
        }

        public async Task<PublicTransportStop> findTheNearestStation(double Lat, double Lon)
        {
            // Implement the logic to find the nearest station
            // This could involve calling other services or APIs
            // For example, you might call the FindTheNearestStationService here
            var nearestStation = await _findTheNearestStationService.FindTheNearestStationServiceAsync(Lat, Lon);
            return nearestStation;
        }
        //public async Task<WalkingRoute> generateRouteToStation(double fromLat, double fromLon, double toLat, double toLon)
        public async Task<(WalkingRoute obj, string jsonResponse)> generateRouteToStation(double fromLat, double fromLon, double toLat, double toLon)
        {
            // Implement the logic to generate a route to the station
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            var (obj, jsonResponse) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);

            return (obj, jsonResponse);
        }

        public async Task<TransitRoute> generateTransitRoute(PublicTransportStop stop1, PublicTransportStop stop2)
        //public async Task<string> generateTransitRoute(PublicTransportStop stop1, PublicTransportStop stop2)
        {
            
            var part2 = await _transitRouteService.CalculateTransitRouteAsync(stop1.Location.Latitude, stop1.Location.Longitude,
                                                                                 stop2.Location.Latitude, stop2.Location.Longitude);

            return part2;
        }

        //public async Task<WalkingRoute> generateRouteToEndPoint(double fromLat, double fromLon, double toLat, double toLon)
        public async Task<(WalkingRoute obj, string jsonResponse)> generateRouteToEndPoint(double fromLat, double fromLon, double toLat, double toLon)
        {
            // Implement the logic to generate a route to the station
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            var (obj, jsonResponse) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);

            return (obj, jsonResponse);
        }

        public async Task<string> generateHybridRoute(double fromLat, double fromLon, double toLat, double toLon)
        {
            // Implement the logic to generate a hybrid route
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            // Find the nearest station
            var nearestStation1 = await findTheNearestStation(fromLat, fromLon);
            Console.WriteLine(nearestStation1.Name + "Lat" + nearestStation1.Location.Latitude + "Long" + nearestStation1.Location.Longitude);
            var nearestStation2 = await findTheNearestStation(toLat, toLon);
            Console.WriteLine(nearestStation2.Name + "Lat" + nearestStation2.Location.Latitude + "Long" + nearestStation2.Location.Longitude);

            // Generate the route to the station
            var (part1, jresponse1) = await generateRouteToStation(fromLat, fromLon, nearestStation1.Location.Latitude, nearestStation1.Location.Longitude);
            //Decode the polyline for the first walking part
            var decodedPoints1 = PolylineDecoder.Decode(part1.Path[0].points);

            //Generate the transit route
            var part2 = await generateTransitRoute(nearestStation1, nearestStation2);
            Console.WriteLine(part2);
            string part4 = JsonSerializer.Serialize(part2);

            //Generate the route to the endpoint
            var (part3, jresponse2) = await generateRouteToEndPoint(nearestStation2.Location.Latitude, nearestStation2.Location.Longitude, toLat, toLon);
            //Decode the polyline for the second walking part

            var decodedPoints2 = PolylineDecoder.Decode(part3.Path[0].points);

            // Create the final response object
            TransitRouteResponse transitRouteResponse = new TransitRouteResponse
            {
                RouteId = "RouteId",
                Start = new List<double> { decodedPoints1[0].Latitude, decodedPoints1[0].Longitude },
                End = new List<double> { decodedPoints2[decodedPoints2.Count - 1].Latitude, decodedPoints2[decodedPoints2.Count - 1].Longitude },
                DistanceMeters = 0,
                DurationSeconds = 0,
                WalkToTransportPolyline = decodedPoints1.Select(point => new List<double> { point.Latitude, point.Longitude }).ToList(),
                TransportPolyline = part2.routes.SelectMany(route => route.legs.SelectMany(leg => leg.stopovers.Select(stopover => new List<double> { stopover.latitude, stopover.longitude }))).ToList(),
                WalkFromTransportPolyline = decodedPoints2.Select(point => new List<double> { point.Latitude, point.Longitude }).ToList(),
                TransportType = part2.routes.SelectMany(route => route.legs).FirstOrDefault(leg => leg.type != "walking")?.type ?? "Unknown",
                TransportLine = part2.routes.SelectMany(route => route.legs).Where(leg => leg.type != "walking" && !string.IsNullOrEmpty(leg.line)).Select(leg => leg.line).FirstOrDefault() ?? "Unknown",
                FromStop = nearestStation1.Name,
                ToStop = nearestStation2.Name
            };

            // Serelialize the final response object
            string finalResponse = JsonSerializer.Serialize(transitRouteResponse);

            //string result = JsonSerializer.Serialize(part1) + JsonSerializer.Serialize(part2) + JsonSerializer.Serialize(part3);

            //return $"{part1} + {part4} + {part3}";
            return finalResponse;
        }
    }
    
}
