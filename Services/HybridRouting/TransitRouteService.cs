// Services/TransitRouteService.cs
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Services
{
    public class TransitRouteService
    {
        private readonly HttpClient _httpClient;

        public TransitRouteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //public async Task<string> CalculateTransitRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
        public async Task<TransitRoute> CalculateTransitRouteAsync(double fromLat, double fromLon, double toLat, double toLon, string deptime)
        {
            try
            {
                string url = $"http://tubify-external-api:8000/api/routes?from={fromLat},{fromLon}&to={toLat},{toLon}&stopovers=true&polylines=true";
            
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                
                // Deserialize the JSON response if needed
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                };
                var route = JsonSerializer.Deserialize<TransitRoute>(jsonResponse, options);
                if (route != null)
                {
                    //return JsonSerializer.Serialize(route); // Return the serialized path
                    Console.WriteLine("Route found! Route length:" + route.routes[0].PrintRouteDetails());
                    //return "Serialized:" + JsonSerializer.Serialize(route);
                    return route;
                }
                else
                {
                    Console.WriteLine("No route found.");
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON Error: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected Error: {e.Message}");
                return null;
            }
        }
    }
}
