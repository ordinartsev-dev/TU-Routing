// Services/GraphHopperService.cs
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Backend.Models;
using System.Text.Json.Serialization;

namespace Backend.Services
{
    public class GraphHopperService
    {
        private readonly HttpClient _httpClient;

        public GraphHopperService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(WalkingRoute path, string finalResponse)> GetRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
        //public async Task<WalkingRoute> GetRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
        {
            try
            {
                string url = $"http://localhost:8989/route?point={fromLat},{fromLon}&point={toLat},{toLon}&vehicle=foot&locale=ru&instructions=true";
                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response if needed
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                };

                var path = JsonSerializer.Deserialize<WalkingRoute>(jsonResponse, options);
                if (path != null)
                {
                    Console.WriteLine("Route found! Route length:" + path.PrintLength());
                    // Decode the route points
                    var decodedPoints = PolylineDecoder.Decode(path.Path[0].points);

                    RouteResponce FootRouting = new RouteResponce
                    {
                        RouteId = "RouteId",
                        Start = new List<double> { decodedPoints[0].Latitude, decodedPoints[0].Longitude },
                        End = new List<double> { decodedPoints[decodedPoints.Count - 1].Latitude, decodedPoints[decodedPoints.Count - 1].Longitude },
                        DistanceMeters = path.Path[0].distance,
                        DurationSeconds = (int)path.Path[0].time,
                        Polyline = decodedPoints.Select(point => new List<double> { point.Latitude, point.Longitude }).ToList()
                    };

                    string finalResponse = string.Empty;

                    finalResponse = JsonSerializer.Serialize(FootRouting);

                    return (path, finalResponse); // Return the serialized path

                    //return path;
                }
                else
                {
                    Console.WriteLine("No stops found.");
                    return (null, string.Empty);
                }

                
                
                
                //return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                // Handle the exception as needed
                Console.WriteLine($"Error: {e.Message}");
                return (null, string.Empty);
            }
            catch (JsonException e)
            {
                // Handle JSON deserialization errors
                Console.WriteLine($"JSON Error: {e.Message}");
                return (null, string.Empty);
            }
            catch (Exception e)
            {
                // Handle other exceptions
                Console.WriteLine($"Unexpected Error: {e.Message}");
                return (null, string.Empty);
            }
        }
    }
}

