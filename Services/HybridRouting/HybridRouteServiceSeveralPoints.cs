using Backend.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Services
{
    public class HybridRouteServiceSeveralPoints
    {
        private readonly HybridRouteService _hybridRouteService;
        public HybridRouteServiceSeveralPoints(GraphHopperService graphHopperService, TransitRouteService transitRouteService, FindTheNearestStationService findTheNearestStationService, HybridRouteService hybridRouteService)
        {
           _hybridRouteService = hybridRouteService;
        }

        public async Task<string> HybridRouteSeveralPointsAsync(List<List<double>> points)
        {
            if (points == null || points.Count < 2)
            {
                return "Invalid points provided. At least two points are required.";
            }

            var route = new List<TransitRouteResponse>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                var fromLat = points[i][0];
                var fromLon = points[i][1];
                var toLat = points[i + 1][0];
                var toLon = points[i + 1][1];

                var temp = await _hybridRouteService.generateHybridRoute(fromLat, fromLon, toLat, toLon);
                if (temp == null)
                {
                    return "No route found.";
                }
                route.Add(JsonSerializer.Deserialize<TransitRouteResponse>(temp)!);
            }

            return JsonSerializer.Serialize(route);
        }
    }
}