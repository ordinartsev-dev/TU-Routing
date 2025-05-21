// Services/TransitRouteService.cs
using System.Net.Http;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class TransitRouteService
    {
        private readonly HttpClient _httpClient;

        public TransitRouteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CalculateTransitRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
        {
            string url = $"http://localhost:8000/api/routes?from={fromLat},{fromLon}&to={toLat},{toLon}";
            
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
