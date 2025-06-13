using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Services
{
    public class WalkingRouteService
    {
        private readonly GraphHopperService _graphHopperService;

        public WalkingRouteService(HttpClient httpClient, GraphHopperService graphHopperService, FindScooterService findScooterService)
        {
            _graphHopperService = graphHopperService;
        }

        public async Task<string> FindWalkingRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
        {
            var (route, response) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);
            if (route == null)
            {
                return "No route found.";
            }

            return response;
        }


        public async Task<string> WalkingRouteAsync(List<List<double>> points)
        {
            if (points == null || points.Count < 2)
            {
                return "Invalid points provided. At least two points are required.";
            }

            string response = string.Empty;

            for (int i = 0; i < points.Count - 1; i++)
            {
                var fromLat = points[i][0];
                var fromLon = points[i][1];
                var toLat = points[i + 1][0];
                var toLon = points[i + 1][1];

                var routeResponse = await FindWalkingRouteAsync(fromLat, fromLon, toLat, toLon);
                if (routeResponse == null)
                {
                    return "No route found.";
                }

                response += routeResponse;
            }

            return response;
        }
    }
}