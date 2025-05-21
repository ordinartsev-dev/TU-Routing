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

        public async Task<string> GetRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
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
                    return JsonSerializer.Serialize(path); // Return the serialized path
                }
                else
                {
                    Console.WriteLine("No stops found.");
                    return null;
                }
                
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                // Handle the exception as needed
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
            catch (JsonException e)
            {
                // Handle JSON deserialization errors
                Console.WriteLine($"JSON Error: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                // Handle other exceptions
                Console.WriteLine($"Unexpected Error: {e.Message}");
                return null;
            }
            /*
            string url = $"http://localhost:8989/route?point={fromLat},{fromLon}&point={toLat},{toLon}&vehicle=foot&locale=ru&instructions=true";
            
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
            */
        }
    }
}
