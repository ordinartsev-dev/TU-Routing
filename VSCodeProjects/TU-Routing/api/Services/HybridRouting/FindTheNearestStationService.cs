// Services/TransitRouteService.cs
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Services
{
    public class FindTheNearestStationService
    {
        private readonly HttpClient _httpClient;

        public FindTheNearestStationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PublicTransportStop> FindTheNearestStationServiceAsync(double Lat, double Lon)
        {
            try 
            {
                string url = $"https://v6.bvg.transport.rest/locations/nearby?latitude={Lat}&longitude={Lon}&results=3";
                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                

                string jsonResponse = await response.Content.ReadAsStringAsync();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                };

                var stops = JsonSerializer.Deserialize<PublicTransportStop[]>(jsonResponse, options);
                if (stops != null && stops.Length > 0)
                {
                    Console.WriteLine(stops[0].PrintName());
                    return stops[0]; // Return the first stop
                }
                else
                {
                    Console.WriteLine("No stops found.");
                    return null;
                }
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
                // Handle any other exceptions
                Console.WriteLine($"Unexpected Error: {e.Message}");
                return null;
            }
        }
    }
    
    
}
