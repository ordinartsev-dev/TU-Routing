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

        public async Task<string> ScooterRouteAsync(List<List<double>> points)
        //public async Task<TransitRoute> CalculateScooterRouteAsync(double Lat, double Lon)
        {
            if (points == null || points.Count < 2)
            {
                return "Invalid points provided. At least two points are required.";
            }

            string response = string.Empty;

            var fromLat = points[0][0];
            var fromLon = points[0][1];
            var scooters = await _findScooterService.FindScooterAsync(fromLat, fromLon);
            var firstPart = await findRouteToScooter(fromLat, fromLon, scooters.bike[0].lat, scooters.bike[0].lon);

            response += firstPart;

            if (firstPart == null)
            {
                return "No route found to the scooter.";
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                int toIndex = i + 1;
                var fromLatitude = points[i][0];
                var fromLongitude = points[i][1];
                var toLatitude = points[toIndex][0];
                var toLongitude = points[toIndex][1];
                var temp = await findRouteToEndPoint(fromLatitude, fromLongitude, toLatitude, toLongitude);

                if (temp == null)
                {
                    return "No route found to the endpoint.";
                }

                response += temp;
            }

            //var secondPart = await findRouteToEndPoint(scooters.bike[0].lat, scooters.bike[0].lon, toLat, toLon);
            return response;

            
        }
    }
}
