// Services/TransitRouteService.cs
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Services
{
    public class FindScooterService
    {
        private readonly HttpClient _httpClient;

        public FindScooterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //public async Task<string> FindScooterAsync(double Lat, double Lon)
        public async Task<BikeResponse> FindScooterAsync(double Lat, double Lon)
        {
            try
            {
                string url = $"http://localhost:8000/api/bikes/nearby?coords={Lat},{Lon}&radius=500&limit=10";
            
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                //return jsonResponse;
                

                // Deserialize the JSON response if needed
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                };
                var bikes = JsonSerializer.Deserialize<BikeResponse>(jsonResponse, options);
                if (bikes != null)
                {
                    //return JsonSerializer.Serialize(route); // Return the serialized path
                    //Console.WriteLine("Route found! Route length:" + bikes.[0.PrintRouteDetails());
                    //return "Serialized:" + JsonSerializer.Serialize(bikes);
                    return bikes;
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
