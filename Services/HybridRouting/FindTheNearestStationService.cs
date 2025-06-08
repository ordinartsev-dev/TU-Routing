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

        public async Task<PublicTransportStop[]> FindTheNearestStationServiceAsync(double Lat, double Lon)
        {
            try 
            {
                string url = $"http://localhost:8000/api/nearest-stations?coords={Lat},{Lon}&results=2";
                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                

                string jsonResponse = await response.Content.ReadAsStringAsync();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                };

                var stopsDeserialized = JsonSerializer.Deserialize<PublicTransportStopsResponse>(jsonResponse, options);
                if (stopsDeserialized != null && stopsDeserialized.stops.Length > 0)
                {
                    foreach (var stop in stopsDeserialized.stops)
                    {
                        Console.WriteLine(stop.PrintName());
                    }
                    //Console.WriteLine(stops[0].PrintName());
                    return stopsDeserialized.stops; // Return the first stop
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
