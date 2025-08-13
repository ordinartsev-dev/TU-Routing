// Services/TransitRouteService.cs
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Services
{
    public class ScooterRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly GraphHopperService _graphHopperService;
        private readonly FindScooterService _findScooterService;

        public ScooterRouteService(HttpClient httpClient, GraphHopperService graphHopperService, FindScooterService findScooterService)
        {
            _httpClient = httpClient;
            _graphHopperService = graphHopperService;
            _findScooterService = findScooterService;
        }

        /*
        public async Task<string> findRouteToScooter(double fromLat, double fromLon, double toLat, double toLon)
        {
            var (routeToScooter, response) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);
            if (routeToScooter == null)
            {
                return "No route found to the scooter.";
            }
            //return $"Route to scooter at coordinates ({toLat}, {toLon}) found." + JsonSerializer.Serialize(routeToScooter);
            return response;
        }
        */

        public async Task<string> findRouteToEndPoint(double fromLat, double fromLon, double toLat, double toLon)
        {
            var (routeToEndPoint, response) = await _graphHopperService.GetCyclingRouteAsync(fromLat, fromLon, toLat, toLon);
            if (routeToEndPoint == null)
            {
                return "No route found to the endpoint.";
            }
            //return $"Route to endpoint at coordinates ({toLat}, {toLon}) found." + JsonSerializer.Serialize(routeToEndPoint);
            return response;
        }
        
        public async Task<(Bike, WalkingRoute)> findTheNearestScooterAndRouteToIt(BikeResponse scooters, double fromLat, double fromLon)
        {
            var allWalkingRoutes = new LinkedList<(Bike, WalkingRoute)>();
            double shortestTime = 0.0;
            for (int i = 0; i < scooters.bike.Length; i++)
            {
                var scooter = scooters.bike[i];
                var (walkingRoute, response) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, scooter.lat, scooter.lon);
                if (walkingRoute != null)
                {
                    if (shortestTime == 0.0 || walkingRoute.Path[0].time < shortestTime)
                    {
                        shortestTime = walkingRoute.Path[0].time;
                        allWalkingRoutes.AddFirst((scooter, walkingRoute));
                    }
                    else
                    {
                        allWalkingRoutes.AddLast((scooter, walkingRoute));
                    }
                }
            }
            return allWalkingRoutes.First.Value;
        }
        

        public async Task<string> ScooterRouteAsync(List<List<double>> points)
        //public async Task<TransitRoute> CalculateScooterRouteAsync(double Lat, double Lon)
        {
            if (points == null || points.Count < 2)
            {
                return "Invalid points provided. At least two points are required.";
            }
            var route = new List<RouteResponce>();

            var fromLat = points[0][0];
            var fromLon = points[0][1];
            var scooters = await _findScooterService.FindScooterAsync(fromLat, fromLon);
            //var firstPart = await findRouteToScooter(fromLat, fromLon, scooters.bike[0].lat, scooters.bike[0].lon);

            var (foundScooter, firstPart) = await findTheNearestScooterAndRouteToIt(scooters, fromLat, fromLon);
            var serializedFirstPart = JsonSerializer.Serialize(firstPart);

            route.Add(JsonSerializer.Deserialize<RouteResponce>(serializedFirstPart)!);

            //route.Add(firstPart)

            if (firstPart == null)
            {
                return "No route found to the scooter.";
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                int toIndex = i + 1;

                var fromLatitude = 0.0;
                var fromLongitude = 0.0;
                
                if (i == 0)
                {
                    fromLatitude = scooters.bike[0].lat;
                    fromLongitude = scooters.bike[0].lon;
                }
                else
                {
                    fromLatitude = points[i][0];
                    fromLongitude = points[i][1];
                }
                
                var toLatitude = points[toIndex][0];
                var toLongitude = points[toIndex][1];
                var temp = await findRouteToEndPoint(fromLatitude, fromLongitude, toLatitude, toLongitude);

                if (temp == null)
                {
                    return "No route found to the endpoint.";
                }
                route.Add(JsonSerializer.Deserialize<RouteResponce>(temp)!);
            }

            //var secondPart = await findRouteToEndPoint(scooters.bike[0].lat, scooters.bike[0].lon, toLat, toLon);
            return JsonSerializer.Serialize(route);

            
        }
    }
}
