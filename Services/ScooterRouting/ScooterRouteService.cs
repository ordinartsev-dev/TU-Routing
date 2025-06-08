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

        public async Task<string> ScooterRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
        //public async Task<TransitRoute> CalculateScooterRouteAsync(double Lat, double Lon)
        {
            var scooters = await _findScooterService.FindScooterAsync(fromLat, fromLon);
            var firstPart = await findRouteToScooter(fromLat, fromLon, scooters.bike[0].lat, scooters.bike[0].lon);
            if (firstPart == null)
            {
                return "No route found to the scooter.";
            }
            var secondPart = await findRouteToEndPoint(scooters.bike[0].lat, scooters.bike[0].lon, toLat, toLon);
            if (secondPart == null)
            {
                return "No route found to the endpoint.";
            }
            return firstPart + secondPart;

            
        }
    }
}
